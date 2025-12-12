using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ChatServer
{
    public static class ServerLogger
    {
        private static readonly string LogFilePath = "server_log.txt";
        private static readonly object _lock = new object();
    }
}
