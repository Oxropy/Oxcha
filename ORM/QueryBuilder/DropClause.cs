using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.QueryBuilder
{
    public class DropClause : IQueryPart
    {
        public readonly string Name;

        public DropClause(string name)
        {
            Name = name;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append("DROP TABLE ");
            sb.Append(Name);
        }
    }
}
