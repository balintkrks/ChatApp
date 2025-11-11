using Microsoft.Data.Sqlite;
using ChatCommon.Models;
using System;
using System.Collections.Generic;

namespace ChatServer.Data
{
    public static class Database
    {
        private const string ConnectionString = "Data Source=chat.db";

        public static void Init()
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Username TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS Messages (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Sender TEXT NOT NULL,
                    Content TEXT NOT NULL,
                    Timestamp TEXT NOT NULL
                );
            ";
            cmd.ExecuteNonQuery();
        }

        public static void AddMessage(string sender, string content)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Messages (Sender, Content, Timestamp) VALUES ($s, $c, $t)";
            cmd.Parameters.AddWithValue("$s", sender);
            cmd.Parameters.AddWithValue("$c", content);
            cmd.Parameters.AddWithValue("$t", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        public static bool AddUser(string username, string passwordHash)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Users (Username, PasswordHash) VALUES ($u, $p)";
            cmd.Parameters.AddWithValue("$u", username);
            cmd.Parameters.AddWithValue("$p", passwordHash);

            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool ValidateUser(string username, string passwordHash)
        {
            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username=$u AND PasswordHash=$p";
            cmd.Parameters.AddWithValue("$u", username);
            cmd.Parameters.AddWithValue("$p", passwordHash);

            long count = (long)cmd.ExecuteScalar();
            return count > 0;
        }

        
        public static List<ChatMessage> GetLastMessages(int count)
        {
            var messages = new List<ChatMessage>();

            using var conn = new SqliteConnection(ConnectionString);
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT Sender, Content, Timestamp FROM Messages ORDER BY Id DESC LIMIT $c";
            cmd.Parameters.AddWithValue("$c", count);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                messages.Add(new ChatMessage
                {
                    Sender = reader.GetString(0),
                    Content = reader.GetString(1),
                    Timestamp = DateTime.Parse(reader.GetString(2))
                });
            }

            messages.Reverse(); 
            return messages;
        }
    }
}
