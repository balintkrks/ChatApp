using System.Net;
using System.Net.Sockets;
using ChatCommon;
using ChatServer.Data; 

class Program
{
    static async Task Main()
    {
       
        Database.Init();

       
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
        string username = "Anon";

        while (true)
        {
            var msg = await Protocol.ReceiveMessageAsync(stream);
            if (msg == null) break;

           
            if (msg.StartsWith("USERNAME:"))
            {
                username = msg.Substring("USERNAME:".Length);
                await Protocol.SendMessageAsync(stream, $"SERVER: Username set to {username}");
                continue;
            }

            
            Database.AddMessage(username, msg);
            string broadcast = $"{username}: {msg}";
            Console.WriteLine(broadcast);

            
            await Protocol.SendMessageAsync(stream, $"Echo: {msg}");
        }

        client.Close();
        Console.WriteLine("Kliens bontotta.");
    }

}
