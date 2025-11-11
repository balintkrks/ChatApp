using System.Net;
using System.Net.Sockets;
using ChatCommon;

class Program
{
    static async Task Main()
    {
        var listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        Console.WriteLine("Szerver fut a 5000-es porton.");

        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            _ = HandleClient(client);
        }
    }

    static async Task HandleClient(TcpClient client)
    {
        Console.WriteLine("Kliens csatlakozott.");
        var stream = client.GetStream();

        while (true)
        {
            var msg = await Protocol.ReceiveMessageAsync(stream);
            if (msg == null) break;
            Console.WriteLine($"Kaptam: {msg}");
            await Protocol.SendMessageAsync(stream, $"Echo: {msg}");
        }

        client.Close();
        Console.WriteLine("Kliens bontotta.");
    }
}
