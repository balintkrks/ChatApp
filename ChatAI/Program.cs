using System;
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

        static async Task Main(string[] args)
        {
            _ai = new AiService();
            Console.WriteLine("=== ChatAI Bot ===");
            await ConnectAsync();
        }

        private static async Task ConnectAsync()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Csatlakozás...");
                    _client = new TcpClient();
                    await _client.ConnectAsync("127.0.0.1", 5000);
                    _stream = _client.GetStream();

                    await Protocol.SendMessageAsync(_stream, "REGISTER:ChatBot:secret");
                    await Task.Delay(500);
                    await Protocol.SendMessageAsync(_stream, "LOGIN:ChatBot:secret");

                    Console.WriteLine("Sikeres belépés!");
                    await ReceiveLoop();
                }
                catch
                {
                    Console.WriteLine("Hiba, újrapróbálkozás 5mp múlva...");
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

                if (msg.StartsWith("USERS:") || msg.StartsWith("FILE:") || msg.Contains("ChatBot")) continue;

                if (msg.StartsWith("(privát)"))
                {
                    var parts = msg.Split(':', 2);
                    string sender = parts[0].Replace("(privát)", "").Trim();
                    string content = parts.Length > 1 ? parts[1].Trim() : "";

                    Console.WriteLine($"Privát: {sender}: {content}");

                    string reply = await _ai.GetAnswerAsync(content);
                    await Protocol.SendMessageAsync(_stream, $"PRIVATE:{sender}:{reply}");
                }
                else
                {
                    if (msg.Contains(":"))
                    {
                        var parts = msg.Split(':', 2);
                        if (parts.Length < 2) continue;

                        string senderInfo = parts[0];
                        string content = parts[1].Trim();
                        string actualSender = senderInfo;
                        if (senderInfo.Contains(" ")) actualSender = senderInfo.Split(' ').Last();

                        if (_ai.ContainsBadWord(content))
                        {
                            Console.WriteLine($"Moderálás: {actualSender}");
                            await Protocol.SendMessageAsync(_stream, $"Kérlek {actualSender}, ne beszélj csúnyán!");
                        }
                    }
                }
            }
        }
    }
}