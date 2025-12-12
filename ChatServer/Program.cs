using ChatCommon;
using ChatCommon.Utils;
using ChatServer;
using ChatServer.Data;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    private static readonly ConcurrentDictionary<TcpClient, NetworkStream> Clients = new();
    private static readonly ConcurrentDictionary<string, TcpClient> UserClients = new();

    static async Task Main()
    {
        Database.Init();
        var listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        ServerLogger.Log("A szerver elindult az 5000-es porton.", "SYSTEM");

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
        string clientIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
        ServerLogger.Log($"Új kapcsolat érkezett innen: {clientIp}", "SYSTEM");
        string username = "Anon";
        bool isLoggedIn = false;

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
                    isLoggedIn = true;

                    await Protocol.SendMessageAsync(stream, $"SERVER: Login successful (Anon)");
                    await BroadcastUserList();
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
                            isLoggedIn = true;

                            await Protocol.SendMessageAsync(stream, "SERVER: Login successful");
                            await BroadcastUserList();

                            var history = Database.GetLastMessages(20);
                            foreach (var m in history)
                            {
                                await Protocol.SendMessageAsync(stream, $"{m.Timestamp:HH:mm} {m.Sender}: {m.Content}");
                            }
                            ServerLogger.Log($"Sikeres bejelentkezés: {username} ({clientIp})", "LOGIN");
                        }
                        else
                        {
                            ServerLogger.Log($"Sikertelen bejelentkezési kísérlet: {user} ({clientIp}) - Rossz jelszó", "ERROR");
                            await Protocol.SendMessageAsync(stream, "SERVER: Login failed");
                        }
                    }
                    continue;
                }

                if (msg.StartsWith("KICK:"))
                {
                    Console.WriteLine($"[DEBUG] KICK command received from {username}: {msg}");
                    var parts = msg.Split(':', 2);
                    if (parts.Length == 2)
                    {
                        string targetUser = parts[1].Trim();
                        Console.WriteLine($"[DEBUG] Target user to kick: {targetUser}");

                        if (username.StartsWith("ChatBot"))
                        {
                            if (UserClients.TryGetValue(targetUser, out var targetClient))
                            {
                                Console.WriteLine($"[DEBUG] User {targetUser} found. Executing kick.");
                                try
                                {
                                    var targetStream = targetClient.GetStream();
                                    await Protocol.SendMessageAsync(targetStream, "SERVER: You have been kicked by the ChatBot.");
                                }
                                catch { }

                                targetClient.Close();
                                UserClients.TryRemove(targetUser, out _);
                                Clients.TryRemove(targetClient, out _);

                                await BroadcastUserList();
                                string kickMsg = $"SERVER: {targetUser} has been kicked.";
                                Console.WriteLine(kickMsg);

                                foreach (var kv in Clients)
                                {
                                    try { await Protocol.SendMessageAsync(kv.Value, kickMsg); } catch { }
                                }
                            }
                            else
                            {
                                Console.WriteLine($"[DEBUG] FAILED: User {targetUser} not found in active clients.");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[DEBUG] FAILED: Permission denied for {username}. Only ChatBot can kick.");
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

                            try
                            {
                                var recipientStream = recipientClient.GetStream();
                                string formattedMsg = $"(privát) {username}: {message}";
                                await Protocol.SendMessageAsync(recipientStream, formattedMsg);
                            }
                            catch { }

                            ServerLogger.Log($"{username} -> {recipient}: {message}", "PRIVATE");
                            var recipientStream = recipientClient.GetStream();
                            await Protocol.SendMessageAsync(recipientStream, $"(privát) {username}: {message}");

                        }
                        else
                        {
                            await Protocol.SendMessageAsync(stream, $"SERVER: A felhasználó ({recipient}) nem található vagy offline.");
                        }
                        await Protocol.SendMessageAsync(stream, $"{recipient}: {message}");
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
                            string sizeReadable;
                            if (fileSize > 1024 * 1024)
                                sizeReadable = $"{fileSize / (1024.0 * 1024.0):F2} MB";
                            else
                                sizeReadable = $"{fileSize / 1024.0:F2} KB";

                            if (!string.IsNullOrEmpty(recipient))
                            {
                                ServerLogger.Log($"{username} -> {recipient}: Fájl küldése: '{fileName}' ({sizeReadable})", "FILE");
                            }
                            else
                            {
                                ServerLogger.Log($"{username} -> GLOBAL: Fájl küldése: '{fileName}' ({sizeReadable})", "FILE");
                            }

                            if (!string.IsNullOrEmpty(recipient))
                            {
                                if (UserClients.TryGetValue(recipient, out var recipientClient))
                                {
                                    var rs = recipientClient.GetStream();
                                    await Protocol.SendMessageAsync(rs, $"FILE:{username}:{fileName}:{fileBytes.Length}");
                                    await Protocol.SendBytesAsync(rs, fileBytes);
                                    await Protocol.SendMessageAsync(rs, "FILE_END");
                                }
                                else
                                {
                                    ServerLogger.Log($"HIBA: Nem sikerült kézbesíteni a fájlt '{recipient}' részére (offline).", "ERROR");
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
                                    catch { }
                                }
                            }
                        }
                    }
                    continue;
                }

                Database.AddMessage(username, msg);
                ServerLogger.Log($"{username}: {msg}", "GLOBAL");
                string broadcast = $"{DateTime.Now:HH:mm} {username}: {msg}";
                Console.WriteLine(broadcast);

                foreach (var kv in Clients)
                {
                    try { await Protocol.SendMessageAsync(kv.Value, broadcast); }
                    catch { }
                }
            }
        }
        catch
        {
        }
        finally
        {
            Clients.TryRemove(client, out _);
            if (isLoggedIn && !string.IsNullOrEmpty(username))
            {
                UserClients.TryRemove(username, out _);
                ServerLogger.Log($"{username} kijelentkezett. (IP: {clientIp})", "LOGOUT");
                _ = BroadcastUserList();
            }
            else
            {
                ServerLogger.Log($"Ismeretlen kliens bontotta a kapcsolatot: {clientIp}", "SYSTEM");
            }
            client.Close();
        }
    }

    static async Task BroadcastUserList()
    {
        var users = UserClients.Keys.ToArray();
        string userListMsg = "USERS:" + string.Join(",", users);

        foreach (var kv in Clients)
        {
            try
            {
                await Protocol.SendMessageAsync(kv.Value, userListMsg);
            }
            catch { }
        }
    }
}