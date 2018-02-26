using log4net;
using ORM.Attributes;
using ORM.DataBase;
using ORM.DataContract;
using ORM.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ORM.Dao
{
    public class Dao<T> where T : IDataContract
    {
        private static ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private DB db;
        public readonly DbTableAttribute DbTable;
        public readonly List<DbFieldAttribute> DbFieldList = new List<DbFieldAttribute>();

        public Dao(DB db)
        {
            this.db = db;

            Type type = typeof(T);
            var customAttributes = type.GetCustomAttributes(false);
            DbTable = customAttributes.First(a => a is DbTableAttribute) as DbTableAttribute;

            var prop = type.GetProperties();
            foreach (var item in prop)
            {
                 var dbField = item.GetCustomAttribute<DbFieldAttribute>();
                if (dbField != null)
                {
                    DbFieldList.Add(dbField);
                }
            }
        }

        public bool CreateTable()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("CREATE TABLE IF NOT EXISTS {0} ", DbTable.Name);
                sb.Append(" ( ");

                var e = DbFieldList.GetEnumerator();
                if (e.MoveNext())
                {
                    var c = e.Current;
                    sb.AppendFormat(" {0} {1} ", c.Name, c.Type);

                    while (e.MoveNext())
                    {
                        c = e.Current;
                        sb.AppendFormat(", {0} {1} ", c.Name, c.Type);
                    }
                }

                sb.Append(")");

                //db.Run(sb.ToString(), c => c.ExecuteNonQuery());
                return true;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error: {0}", ex);
                throw;
            }
        }

        public bool Insert(T values)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("INSERT INTO {0} (", DbTable.Name);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error: {0}", ex);
                throw;
            }
            return false;
        }

        public bool Delete(WhereClause where)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                //sb.AppendFormat("DELETE FROM {0} ", dbTable.Name);
                sb.Append(where);

                Console.WriteLine(sb.ToString());
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error: {0}", ex);
                throw;
            }
            return false;
        }

        public List<T> Listing(IEnumerable<string> selectList, WhereClause where)
        {
            return null;
        }

    }
}
