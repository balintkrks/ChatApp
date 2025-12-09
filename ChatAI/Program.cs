using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatCommon;
using ChatAI;

namespace ChatAI
{
    class Program
    {
        private static TcpClient _client;
        private static NetworkStream _stream;
        private static AiService _ai;
        private static Dictionary<string, int> _warnings = new Dictionary<string, int>();

        static async Task Main(string[] args)
        {
            _ai = new AiService();
            Console.WriteLine("=== ChatAI Bot v2 ===");
            await ConnectAsync();
        }

        private static async Task ConnectAsync()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Connecting...");
                    _client = new TcpClient();
                    await _client.ConnectAsync("127.0.0.1", 5000);
                    _stream = _client.GetStream();

                    await Protocol.SendMessageAsync(_stream, "REGISTER:ChatBot:secret");
                    await Task.Delay(500);
                    await Protocol.SendMessageAsync(_stream, "LOGIN:ChatBot:secret");

                    Console.WriteLine("Connected!");
                    await ReceiveLoop();
                }
                catch
                {
                    Console.WriteLine("Connection error, retrying in 5s...");
                    await Task.Delay(5000);
                }
            }
        }

        private static async Task ReceiveLoop()
        {
            while (true)
            {
                string msg = await Protocol.ReceiveMessageAsync(_stream);
                if (msg == null) break;

                if (msg.StartsWith("USERS:") || msg.StartsWith("FILE:") || msg.Contains("ChatBot:")) continue;

                if (msg.StartsWith("(privát)"))
                {
                    var parts = msg.Split(':', 2);
                    string sender = parts[0].Replace("(privát)", "").Trim();
                    string content = parts.Length > 1 ? parts[1].Trim() : "";

                    Console.WriteLine($"Private: {sender}: {content}");

                    string reply = await _ai.GetAnswerAsync(content);
                    await Protocol.SendMessageAsync(_stream, $"PRIVATE:{sender}:{reply}");
                }
                else
                {
                    int separatorIndex = msg.IndexOf(": ");

                    if (separatorIndex > 0)
                    {
                        string header = msg.Substring(0, separatorIndex);
                        string content = msg.Substring(separatorIndex + 2).Trim();

                        string[] headerParts = header.Split(' ');
                        string actualSender = headerParts.Last();

                        if (actualSender == "ChatBot") continue;

                        bool isBad = await _ai.IsOffensiveAsync(content);

                        if (isBad)
                        {
                            if (!_warnings.ContainsKey(actualSender)) _warnings[actualSender] = 0;
                            _warnings[actualSender]++;

                            int count = _warnings[actualSender];
                            Console.WriteLine($"Offense: {actualSender} ({count}/3)");

                            if (count < 3)
                            {
                                await Protocol.SendMessageAsync(_stream, $"FIGYELEM {actualSender}! Ez sértő beszéd volt. ({count}/3 figyelmeztetés)");
                            }
                            else
                            {
                                await Protocol.SendMessageAsync(_stream, $"SAJNÁLOM {actualSender}, 3. figyelmeztetés után kirúgás!");
                                await Task.Delay(1000);
                                await Protocol.SendMessageAsync(_stream, $"KICK:{actualSender}");

                                _warnings.Remove(actualSender);
                            }
                        }
                    }
                }
            }
        }
    }
}