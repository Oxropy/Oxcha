using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.QueryBuilder
{
    //https://docs.microsoft.com/en-us/sql/t-sql/statements/create-type-transact-sql#arguments
    public enum BaseType
    {
        Bigint,
        Binary,
        Bit,
        Char,
        Date,
        DateTime,
        DateTime2,
        DateTimeOffset,
        Decimal,
        Float,
        Image,
        Int,
        Money,
        NChar,
        NText,
        Numeric,
        NVarchar,
        Real,
        SmallDateTime,
        SmallInt,
        SmallMoney,
        Sql_Variant,
        Text,
        Time,
        Tinyint,
        Uniqueidentifier,
        Varbinary,
        Varchar
    }
    
    public class ColumnDefinition : ICreate
    {
        public readonly string Name;
        public readonly BaseType Type;
        public readonly int TypeLength;

        public ColumnDefinition(string name, BaseType type, int typeLength = 0)
        {
            Name = name;
            Type = type;
            TypeLength = typeLength;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append(Name);
            sb.Append(" ");
            sb.Append(GetTypeValue(Type, TypeLength));
        }

        public static string GetTypeValue(BaseType type, int length)
        {
            switch (type)
            {
                case BaseType.Binary:
                case BaseType.Char:
                case BaseType.NChar:
                case BaseType.NVarchar:
                case BaseType.Varbinary:
                case BaseType.Varchar:
                    return string.Format("{0}({1})", type, length);
                default:
                    return type.ToString();
            }
        }
    }

    public class CreateClause : IQueryPart
    {
        public readonly string Name;
        public readonly ICreate create;

        public CreateClause(string name, ICreate create)
        {
            this.Name = name;
            this.create = create;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append("CREATE TABLE ");
            sb.Append(Name);
        }
    }
}
