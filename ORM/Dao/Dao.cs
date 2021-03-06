﻿using log4net;
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
using static ORM.QueryBuilder.QueryBuilderExtensions;

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
                IEnumerable<ICreate> createFields = DbFieldList.Select(f =>  f.Name.ColDefinition(f.Type));
                
                CreateClause create = Create(DbTable.Name, true, createFields);

                string query = create.GetQuery();
                log.Debug(query);

                //db.Run(query, c => c.ExecuteNonQuery());
                return true;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error: {0}", ex);
                throw;
            }
        }

        public bool DropTable()
        {
            try
            {
                DropClause drop = Drop(DbTable.Name);

                string query = drop.GetQuery();
                log.Debug(drop.GetQuery());

                //db.Run(query, c => c.ExecuteNonQuery());

                return true;
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error: {0}", ex);
                throw;
            }
        }

        public bool DeleteRow(WhereClause where)
        {
            try
            {
                DeleteClause delete = Delete(DbTable.Name, where);

                string query = delete.GetQuery();
                log.Debug(delete.GetQuery());

                //db.Run(query, c => c.ExecuteNonQuery());

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

        public List<T> Listing(IEnumerable<string> selectList, WhereClause where)
        {
            return null;
        }

    }
}
