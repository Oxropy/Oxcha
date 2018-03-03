using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.QueryBuilder
{
    public enum BaseType
    {
        Text,
        Numberic,
        Integer
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
            return type.ToString();
        }
    }

    public class CreateClause : IQueryPart
    {
        public readonly string Name;
        public readonly bool IfNotExist;
        public readonly IEnumerable<ICreate> Create;

        public CreateClause(string name, bool ifnotExist, IEnumerable<ICreate> create)
        {
            this.Name = name;
            this.IfNotExist = ifnotExist;
            this.Create = create;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append("CREATE TABLE ");
            if (IfNotExist)
            {
                sb.Append("IF NOT EXISTS ");
            }
            sb.Append(Name);
            sb.Append(" (");
            QueryHelper.BuildJoinedExpression(sb, ", ", Create);
            sb.Append(")");
        }
    }
}
