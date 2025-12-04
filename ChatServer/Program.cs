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

                
                if (msg == "LOGIN_ANON")
                {
                    username = $"Anon_{Guid.NewGuid().ToString().Substring(0, 4)}";
                    UserClients[username] = client;

                    await Protocol.SendMessageAsync(stream, $"SERVER: Login successful (Anon)");
                    Console.WriteLine($"{username} belépett (Anonim).");
                    continue;
                }

           
                if (msg.StartsWith("REGISTER:"))
                {
                    var parts = msg.Split(':', 3, StringSplitOptions.None);
                    if (parts.Length == 3)
                    {
                        string user = parts[1].Trim();
                        string pass = parts[2];
                        string passHash = HashHelper.Sha256(pass);

                        bool ok = Database.AddUser(user, passHash);
                      
                        await Protocol.SendMessageAsync(stream, ok ? "SERVER: Registration successful" : "SERVER: Registration failed");
                    }
                    continue;
                }

              
                if (msg.StartsWith("LOGIN:"))
                {
                    var parts = msg.Split(':', 3, StringSplitOptions.None);
                    if (parts.Length == 3)
                    {
                        string user = parts[1].Trim();
                        string pass = parts[2];
                        string passHash = HashHelper.Sha256(pass);

                        bool ok = Database.ValidateUser(user, passHash);
                        if (ok)
                        {
                            username = user;
                            UserClients[username] = client; 
                            await Protocol.SendMessageAsync(stream, "SERVER: Login successful");

                           
                            Console.WriteLine($"{username} belépett. History küldése...");
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
                    continue;
                }


                if (msg.StartsWith("PRIVATE:"))
                {
                    var parts = msg.Split(':', 3);
                    if (parts.Length == 3)
                    {
                        var recipient = parts[1];
                        var message = parts[2];
                        if (UserClients.TryGetValue(recipient, out var recipientClient))
                        {
                            var recipientStream = recipientClient.GetStream();
                           
                            await Protocol.SendMessageAsync(recipientStream, $"(privát) {username}: {message}");
                        }
                    }
                    continue;
                }

               
                if (msg.StartsWith("FILE:"))
                {
                    var parts = msg.Split(':', 4); 
                    if (parts.Length == 4)
                    {
                        var recipient = parts[1];
                        var fileName = parts[2];
                        var fileSize = int.Parse(parts[3]);

                        var fileBytes = await Protocol.ReceiveBytesAsync(stream);
                        await Protocol.ReceiveMessageAsync(stream);

                        if (fileBytes != null && fileBytes.Length == fileSize)
                        {
                            Console.WriteLine($"Fájl érkezett {username}-től: {fileName}");

                            if (!string.IsNullOrEmpty(recipient))
                            {
                               
                                if (UserClients.TryGetValue(recipient, out var recipientClient))
                                {
                                    var rs = recipientClient.GetStream();
                                    
                                    await Protocol.SendMessageAsync(rs, $"FILE:{username}:{fileName}:{fileBytes.Length}");
                                    await Protocol.SendBytesAsync(rs, fileBytes);
                                    await Protocol.SendMessageAsync(rs, "FILE_END");
                                }
                            }
                            else
                            {
                                
                                foreach (var kv in Clients)
                                {
                                    try
                                    {
                                        await Protocol.SendMessageAsync(kv.Value, $"FILE:{username}:{fileName}:{fileBytes.Length}");
                                        await Protocol.SendBytesAsync(kv.Value, fileBytes);
                                        await Protocol.SendMessageAsync(kv.Value, "FILE_END");
                                    }
                                    catch {  }
                                }
                            }
                        }
                    }
                    continue;
                }

                Database.AddMessage(username, msg);

                string broadcast = $"{DateTime.Now:HH:mm} {username}: {msg}";
                Console.WriteLine(broadcast);

                foreach (var kv in Clients)
                {
                    try { await Protocol.SendMessageAsync(kv.Value, broadcast); }
                    catch { }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Hiba: {ex.Message}");
        }
        finally
        {
            Clients.TryRemove(client, out _);
            if (username != "Anon")
            {
                UserClients.TryRemove(username, out _);
            }
            client.Close();
            Console.WriteLine("Kliens lecsatlakozott.");
        }
    }
}