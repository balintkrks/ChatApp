using System.Net;
using System.Net.Sockets;
using ChatCommon;
using System.Collections.Concurrent;

class Program
{
    static TcpListener listener;
    static ConcurrentDictionary<int, TcpClient> clients = new();
    static int clientIdSeq = 0;

    static async Task Main()
    {
        listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Szerver fut a 5000-es porton.");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            int id = Interlocked.Increment(ref clientIdSeq);
            clients[id] = client;
            Console.WriteLine($"Kliens csatlakozott (id={id}).");

            _ = HandleClient(id, client);
        }
    }

    static async Task HandleClient(int id, TcpClient client)
    {
        var stream = client.GetStream();
        string username = $"User{id}";

        while (true)
        {
            var msg = await Protocol.ReceiveMessageAsync(stream);
            if (msg == null) break;

            
            if (msg.StartsWith("USERNAME:"))
            {
                username = msg.Substring("USERNAME:".Length).Trim();
                Console.WriteLine($"id={id} beállította a nevét: {username}");
                await Protocol.SendMessageAsync(stream, $"SERVER: Üdv, {username}!");
                continue;
            }

          
            string broadcast = $"{username}: {msg}";
            Console.WriteLine(broadcast);
            await BroadcastAsync(broadcast, excludeId: id);
        }

        clients.TryRemove(id, out _);
        client.Close();
        Console.WriteLine($"Kliens bontotta (id={id}).");
    }

    static async Task BroadcastAsync(string message, int? excludeId = null)
    {
        foreach (var kv in clients)
        {
            if (excludeId.HasValue && kv.Key == excludeId.Value) continue;
            var c = kv.Value;
            try
            {
                await Protocol.SendMessageAsync(c.GetStream(), message);
            }
            catch
            {
                
            }
        }
    }
}
