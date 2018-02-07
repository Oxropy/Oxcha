using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace Twitchat.Logic.IO
{
    public class DB
    {
        private readonly SQLiteConnection connection;

        public DB(string fileName)
        {
            connection = new SQLiteConnection("Data Source=" + fileName + ";Version=3;");
            Initialize();
        }

        public T Run<T>(string query, Func<SQLiteCommand, T> f)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            using (var cmd = new SQLiteCommand(query, connection))
            {
                return f(cmd);
            }
        }

        public void Run(string query, Action<SQLiteCommand> action)
        {
            Run(query, (c) =>
            {
                action(c);
            });
        }

        private void Initialize()
        {
            // Twitch Token
            Run("CREATE TABLE IF NOT EXISTS user_auth_key (user string, access_token string)", c => c.ExecuteNonQuery());

            // Autojoin Channel
            Run("CREATE TABLE IF NOT EXISTS channelautojoin (id int AUTO_INCREMENT, user string, channel string, PRIMARY KEY (id))", c => c.ExecuteNonQuery());

            // Friend Channel
            Run("CREATE TABLE IF NOT EXISTS friends (id int AUTO_INCREMENT, user string, alias string, PRIMARY KEY (id))", c => c.ExecuteNonQuery());
        }
    }
}
