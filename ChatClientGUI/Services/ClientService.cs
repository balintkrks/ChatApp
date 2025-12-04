using System;
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

        private async Task ReceiveLoop()
        {
            try
            {
                while (_isConnected)
                {
                    string msg = await Protocol.ReceiveMessageAsync(_stream);
                    if (msg == null) break;

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