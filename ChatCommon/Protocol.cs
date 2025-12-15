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
            int got = 0;
            while (got < 4)
            {
                int r = await stream.ReadAsync(lenBuf, got, 4 - got);
                if (r == 0) return null; 
                got += r;
            }

            int len = BitConverter.ToInt32(lenBuf, 0);
            if (len <= 0) return string.Empty;

            var buf = new byte[len];
            int read = 0;
            while (read < len)
            {
                int r = await stream.ReadAsync(buf, read, len - read);
                if (r == 0) return null;
                read += r;
            }

            return Encoding.UTF8.GetString(buf);
        }
        public static async Task SendBytesAsync(Stream stream, byte[] data)
        {
            var len = BitConverter.GetBytes(data.Length);
            await stream.WriteAsync(len, 0, len.Length);
            await stream.WriteAsync(data, 0, data.Length);
        }

        public static async Task<byte[]?> ReceiveBytesAsync(Stream stream)
        {
            var lenBuf = new byte[4];
            int got = 0;
            while (got < 4)
            {
                int r = await stream.ReadAsync(lenBuf, got, 4 - got);
                if (r == 0)
                {
                    return null;
                } 
                got += r;
            }

            int len = BitConverter.ToInt32(lenBuf, 0);
            if (len <= 0)
            {
                return Array.Empty<byte>();
            }

            var buf = new byte[len];
            int read = 0;
            while (read < len)
            {
                int r = await stream.ReadAsync(buf, read, len - read);
                if (r == 0)
                {
                    return null;
                }
                read += r;
            }

            return buf;
        }
    }
}