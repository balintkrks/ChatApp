using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using ChatCommon;

namespace ChatClientGUI.Services
{
    public class ClientService
    {
        private TcpClient _client;
        private NetworkStream _stream;
        private bool _isConnected;

        public event Action<string> MessageReceived;
        public event Action ConnectionLost;
        public event Action<string, byte[]> FileReceived;
        public event Action<string[]> UserListReceived;

        public async Task<bool> ConnectAsync(string ip, int port)
        {
            if (_isConnected) return true;

            try
            {
                _client = new TcpClient();
                await _client.ConnectAsync(ip, port);
                _stream = _client.GetStream();
                _isConnected = true;

                _ = ReceiveLoop();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task SendLoginAsync(string username, string password)
        {
            if (!_isConnected) return;
            await Protocol.SendMessageAsync(_stream, $"LOGIN:{username}:{password}");
        }

        public async Task SendLoginAnonAsync()
        {
            if (!_isConnected) return;
            await Protocol.SendMessageAsync(_stream, "LOGIN_ANON");
        }

        public async Task SendRegisterAsync(string username, string password)
        {
            if (!_isConnected) return;
            await Protocol.SendMessageAsync(_stream, $"REGISTER:{username}:{password}");
        }

        public async Task SendMessageAsync(string message)
        {
            if (!_isConnected) return;
            await Protocol.SendMessageAsync(_stream, message);
        }

        public async Task SendPrivateMessageAsync(string recipient, string message)
        {
            if (!_isConnected) return;
            await Protocol.SendMessageAsync(_stream, $"PRIVATE:{recipient}:{message}");
        }

        public async Task SendFileAsync(string filePath, string recipient = "")
        {
            if (!_isConnected) return;

            var fileName = Path.GetFileName(filePath);
            var fileBytes = await File.ReadAllBytesAsync(filePath);

            await Protocol.SendMessageAsync(_stream, $"FILE:{recipient}:{fileName}:{fileBytes.Length}");
            await Protocol.SendBytesAsync(_stream, fileBytes);
            await Protocol.SendMessageAsync(_stream, "FILE_END");
        }

        private async Task ReceiveLoop()
        {
            try
            {
                while (_isConnected)
                {
                    string msg = await Protocol.ReceiveMessageAsync(_stream);
                    if (msg == null) break;

                    if (msg.StartsWith("USERS:"))
                    {
                        string data = msg.Substring(6);
                        string[] users = data.Split(',', StringSplitOptions.RemoveEmptyEntries);
                        UserListReceived?.Invoke(users);
                        continue;
                    }

                    if (msg.StartsWith("FILE:"))
                    {
                        var parts = msg.Split(':', 4);
                        if (parts.Length == 4)
                        {
                            var sender = parts[1];
                            var fileName = parts[2];
                            var fileSize = int.Parse(parts[3]);

                            var fileBytes = await Protocol.ReceiveBytesAsync(_stream);
                            await Protocol.ReceiveMessageAsync(_stream);

                            if (fileBytes != null)
                            {
                                FileReceived?.Invoke(fileName, fileBytes);
                            }
                        }
                        continue;
                    }

                    MessageReceived?.Invoke(msg);
                }
            }
            catch
            {
            }
            finally
            {
                _isConnected = false;
                ConnectionLost?.Invoke();
                _client?.Close();
            }
        }
    }
}