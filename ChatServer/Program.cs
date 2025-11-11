using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using ChatCommon;
using ChatServer.Data;

class Program
{
    private static readonly ConcurrentDictionary<TcpClient, NetworkStream> Clients = new();

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
                if (string.IsNullOrWhiteSpace(msg)) continue;

                if (msg.StartsWith("USERNAME:"))
                {
                    username = msg.Substring("USERNAME:".Length);
                    await Protocol.SendMessageAsync(stream, $"SERVER: Username set to {username}");
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
            Console.WriteLine($"Hiba: {ex.Message}");
        }
        finally
        {
            Clients.TryRemove(client, out _);
            client.Close();
            Console.WriteLine("Kliens bontotta.");
        }
    }
}
