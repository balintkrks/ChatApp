using System.Net.Sockets;
using ChatCommon;
using System.Linq;

class Program
{
    static async Task Main()
    {
        var client = new TcpClient();
        await client.ConnectAsync("127.0.0.1", 5000);
        var stream = client.GetStream();

        Console.Write("Felhasználónév: ");
        string username = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrEmpty(username)) username = "Anon";
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
            if (input == null) break;
            input = input.Trim();
            if (input.Length == 0) continue;

            if (input.Equals("/quit", StringComparison.OrdinalIgnoreCase)) break;

            
            if (input.StartsWith("/register ", StringComparison.OrdinalIgnoreCase))
            {
                var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    var user = parts[1];
                    var pass = string.Join(' ', parts.Skip(2));
                    await Protocol.SendMessageAsync(stream, $"REGISTER:{user}:{pass}");
                }
                else
                {
                    Console.WriteLine("<<< SERVER: Usage: /register <username> <password>");
                }
                continue;
            }

           
            if (input.StartsWith("/login ", StringComparison.OrdinalIgnoreCase))
            {
                var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    var user = parts[1];
                    var pass = string.Join(' ', parts.Skip(2));
                    await Protocol.SendMessageAsync(stream, $"LOGIN:{user}:{pass}");
                }
                else
                {
                    Console.WriteLine("<<< SERVER: Usage: /login <username> <password>");
                }
                continue;
            }

            
            await Protocol.SendMessageAsync(stream, input);
        }

        client.Close();
    }
}
