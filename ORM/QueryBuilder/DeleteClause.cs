using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.QueryBuilder
{
    public class DeleteClause : IQueryPart
    {
        public readonly string Name;
        public readonly WhereClause Where;

        public DeleteClause(string name, WhereClause where)
        {
            Name = name;
            Where = where;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append("DELETE FROM TABLE ");
            sb.Append(Name);
            sb.Append(" ");
            sb.Append(Where.GetQuery());
        }
    }
}
