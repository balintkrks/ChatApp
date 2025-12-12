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

        public static void Log(string message, string type = "INFO")
        {
            ConsoleColor color = ConsoleColor.White;
            switch (type)
            {
                case "LOGIN": 
                    color = ConsoleColor.Green; 
                    break;
                case "LOGOUT": 
                    color = ConsoleColor.Yellow; 
                    break;
                case "ERROR": 
                    color = ConsoleColor.Red; 
                    break;
                case "PRIVATE": 
                    color = ConsoleColor.Cyan; 
                    break;
                case "GLOBAL": 
                    color = ConsoleColor.Gray; 
                    break;
                case "SYSTEM": 
                    color = ConsoleColor.Magenta; 
                    break;
            }

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logLine = $"[{timestamp}] [{type}] {message}";

            lock(_lock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(logLine);
                Console.ResetColor();

                try
                {
                    File.AppendAllText(LogFilePath, logLine + Environment.NewLine);
                }
                catch 
                {

                }
            }
        }
    }
}
