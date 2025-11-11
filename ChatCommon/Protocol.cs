using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatCommon
{
    public static class Protocol
    {
        public static async Task SendMessageAsync(Stream stream, string message)
        {
            var payload = Encoding.UTF8.GetBytes(message);
            var len = BitConverter.GetBytes(payload.Length);
            await stream.WriteAsync(len, 0, len.Length);
            await stream.WriteAsync(payload, 0, payload.Length);
        }

        public static async Task<string?> ReceiveMessageAsync(Stream stream)
        {
            var lenBuf = new byte[4];
            int read = await stream.ReadAsync(lenBuf, 0, 4);
            if (read < 4) return null;
            int len = BitConverter.ToInt32(lenBuf, 0);

            var buf = new byte[len];
            int received = 0;
            while (received < len)
            {
                int r = await stream.ReadAsync(buf, received, len - received);
                if (r == 0) return null;
                received += r;
            }
            return Encoding.UTF8.GetString(buf);
        }
    }
}