using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using ChatCommon;
using ChatCommon.Utils;   
using ChatServer.Data;

class Program
{
    private static readonly ConcurrentDictionary<TcpClient, NetworkStream> Clients = new();
    private static readonly ConcurrentDictionary<string, TcpClient> UserClients = new();

    static async Task Main()
    {
        Database.Init();

        var listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Szerver fut a 5000-es porton.");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            var stream = client.GetStream();
            Clients[client] = stream;
            _ = HandleClient(client, stream);
        }
    }

    static async Task HandleClient(TcpClient client, NetworkStream stream)
    {
        Console.WriteLine("Kliens csatlakozott.");
        string username = "Anon";

        try
        {
            while (true)
            {
                var msg = await Protocol.ReceiveMessageAsync(stream);
                if (msg == null) break;
                msg = msg.Trim();
                if (string.IsNullOrWhiteSpace(msg)) continue;

                
                if (msg.StartsWith("USERNAME:"))
                {
                    username = msg.Substring("USERNAME:".Length).Trim();
                    if (string.IsNullOrEmpty(username)) username = "Anon";
                    await Protocol.SendMessageAsync(stream, $"SERVER: Username set to {username}");
                    continue;
                }

                
                if (msg.StartsWith("REGISTER:"))
                {
                    var parts = msg.Split(':', 3, StringSplitOptions.None);
                    if (parts.Length == 3 && !string.IsNullOrWhiteSpace(parts[1]) && !string.IsNullOrWhiteSpace(parts[2]))
                    {
                        string user = parts[1].Trim();
                        string pass = parts[2]; 
                        string passHash = HashHelper.Sha256(pass);

                        bool ok = Database.AddUser(user, passHash);
                        await Protocol.SendMessageAsync(stream, ok ? "SERVER: Registration successful" : "SERVER: Registration failed (username may be taken)");
                    }
                    else
                    {
                        await Protocol.SendMessageAsync(stream, "SERVER: Invalid REGISTER format (use REGISTER:username:password)");
                    }
                    continue;
                }

                
                if (msg.StartsWith("LOGIN:"))
                {
                    var parts = msg.Split(':', 3, StringSplitOptions.None);
                    if (parts.Length == 3 && !string.IsNullOrWhiteSpace(parts[1]) && !string.IsNullOrWhiteSpace(parts[2]))
                    {
                        string user = parts[1].Trim();
                        string pass = parts[2];
                        string passHash = HashHelper.Sha256(pass);

                        bool ok = Database.ValidateUser(user, passHash);
                        if (ok)
                        {
                            username = user;
                            await Protocol.SendMessageAsync(stream, "SERVER: Login successful");

                            
                            var history = Database.GetLastMessages(20);
                            foreach (var m in history)
                            {
                                await Protocol.SendMessageAsync(stream, $"{m.Timestamp:HH:mm} {m.Sender}: {m.Content}");
                            }
                        }
                        else
                        {
                            await Protocol.SendMessageAsync(stream, "SERVER: Login failed");
                        }
                    }
                    else
                    {
                        await Protocol.SendMessageAsync(stream, "SERVER: Invalid LOGIN format (use LOGIN:username:password)");
                    }
                    continue;
                }

                
                Database.AddMessage(username, msg);
                string broadcast = $"{username}: {msg}";
                Console.WriteLine(broadcast);

                foreach (var kv in Clients)
                {
                    try
                    {
                        await Protocol.SendMessageAsync(kv.Value, broadcast);
                    }
                    catch
                    {
                        Clients.TryRemove(kv.Key, out _);
                        kv.Key.Close();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba a kliensnél: {ex.Message}");
        }
        finally
        {
            Clients.TryRemove(client, out _);
            client.Close();
            Console.WriteLine("Kliens bontotta.");
        }
    }
}
