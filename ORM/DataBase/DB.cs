using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.DataBase
{
    public class DB
    {
        private readonly SQLiteConnection connection;

        public DB(string fileName)
        {
            connection = new SQLiteConnection("Data Source=" + fileName + ";Version=3;");
        }

        public T Run<T>(string query, Func<SQLiteCommand, T> f, IEnumerable<object> parameters = null)
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            using (var cmd = new SQLiteCommand(query, connection))
            {
                if (parameters != null)
                {
                    foreach (var parameter in parameters)
                    {
                        SQLiteParameter sqlParameter = new SQLiteParameter
                        {
                            Value = parameter
                        };

                        cmd.Parameters.Add(sqlParameter);
                    }
                }

                return f(cmd);
            }
        }

        public void Run(string query, Action<SQLiteCommand> action, IEnumerable<object> parameters = null)
        {
            Run(query, c =>
            {
                action(c);
            }, parameters);
        }
    }
}
