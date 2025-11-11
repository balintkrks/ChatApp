using Microsoft.Data.Sqlite;

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
    }
}
