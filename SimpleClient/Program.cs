using System.Net.Sockets;
using ChatCommon;

class Program
{
    static async Task Main()
    {
        var client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", 5000);
        var stream = client.GetStream();

        Console.Write("Felhasználónév: ");
        string username = Console.ReadLine() ?? "Anon";
        await Protocol.SendMessageAsync(stream, $"USERNAME:{username}");

      
      

        _ = Task.Run(async () =>
        {
            while (true)
            {
                var msg = await Protocol.ReceiveMessageAsync(stream);
                if (msg == null) break;
                Console.WriteLine($"<<< {msg}");
            }
        });

     

        while (true)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;
            if (input.Equals("/quit", StringComparison.OrdinalIgnoreCase)) break;

            await Protocol.SendMessageAsync(stream, input);
        }

        client.Close();
    }
}
