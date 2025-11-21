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

                if (msg.StartsWith("FILE:"))
                {
                    var parts = msg.Split(':', 4);
                    if (parts.Length == 4)
                    {
                        var sender = parts[1];
                        var fileName = parts[2];
                        var fileSize = int.Parse(parts[3]);

                        Console.WriteLine($"<<< Fájl érkezik {sender}-től: {fileName} ({fileSize} byte)");

                        var fileBytes = await Protocol.ReceiveBytesAsync(stream);
                        if (fileBytes != null && fileBytes.Length == fileSize)
                        {
                            var savePath = Path.Combine(Environment.CurrentDirectory, "Downloads", fileName);
                            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
                            await File.WriteAllBytesAsync(savePath, fileBytes);

                            Console.WriteLine($"<<< Fájl mentve ide: {savePath}");
                        }
                        else
                        {
                            Console.WriteLine("<<< SERVER: File transfer failed");
                        }
                        var endMsg = await Protocol.ReceiveMessageAsync(stream);
                        if (endMsg == "FILE_END")
                        {
                            Console.WriteLine("<<< Fájl átvitel befejezve");
                        }
                    }
                    continue;
                }
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

            if (input.StartsWith("/msg ", StringComparison.OrdinalIgnoreCase))
            {
                var parts = input.Split(' ', 3, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 3)
                {
                    var recipient = parts[1];
                    var message = parts[2];
                    await Protocol.SendMessageAsync(stream, $"PRIVATE:{recipient}:{message}");
                }
                else
                {
                    Console.WriteLine("<<< SERVER: Usage: /msg <recipient> <message>");
                }
                continue;
            }

            if (input.StartsWith("/sendfile ", StringComparison.OrdinalIgnoreCase))
            {
                var parts = input.Split(' ', 3);
                if (parts.Length >= 2 && File.Exists(parts[1]))
                {
                    var filePath = parts[1];
                    var recipient = parts.Length == 3 ? parts[2] : "";
                    var fileName = Path.GetFileName(filePath);
                    var fileBytes = await File.ReadAllBytesAsync(filePath);

                    await Protocol.SendMessageAsync(stream, $"FILE:{recipient}:{fileName}:{fileBytes.Length}");
                    await Protocol.SendBytesAsync(stream, fileBytes);
                    await Protocol.SendMessageAsync(stream, "FILE_END");
                }
                else
                {
                    Console.WriteLine("<<< SERVER: Fájl nem található");
                }
                continue;
            }
            await Protocol.SendMessageAsync(stream, input);
        }

        client.Close();
    }
}
