using System.Net.Sockets;
using ChatCommon;

class Program
{
    static async Task Main()
    {
        var client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", 5000);
        var stream = client.GetStream();

        Console.WriteLine("Csatlakoztál a szerverhez.");
        Console.WriteLine("Írj be egy üzenetet:");

        while (true)
        {
            string? input = Console.ReadLine();
            if (string.IsNullOrEmpty(input)) continue;
            if (input.Equals("/quit", StringComparison.OrdinalIgnoreCase)) break;

            await Protocol.SendMessageAsync(stream, input);
            var response = await Protocol.ReceiveMessageAsync(stream);
            Console.WriteLine($"<<< {response}");
        }

        client.Close();
    }
}
