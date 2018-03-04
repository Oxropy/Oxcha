using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.QueryBuilder
{

    public class InsertValue : IInsert
    {
        public readonly string Name;
        public readonly object Value;

        public InsertValue(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append(Value);
        }
    }

    public class InsertClause : IQueryPart
    {
        public readonly string Name;
        public readonly IEnumerable<InsertValue> Values;

        public InsertClause(string name, IEnumerable<InsertValue> values)
        {
            Name = name;
            Values = values;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append("INSERT INTO ");
            sb.Append(Name);
            sb.Append(" (");

            #region Fieldname build
            var e = Values.GetEnumerator();
            if (e.MoveNext())
            {
                var v = e.Current;
                sb.Append(v.Name);

                while (e.MoveNext())
                {
                    v = e.Current;
                    sb.Append(", ");
                    sb.Append(v.Name);
                }
            }
            #endregion

            sb.Append(") ");
            sb.Append("  VALUES ");
            sb.Append(" (");
            QueryHelper.BuildJoinedExpression(sb, ", ", Values);
            sb.Append(")");
        }
    }
}
