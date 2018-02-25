using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.QueryBuilder
{
    
    public class FieldAliasExpression : ISelection
    {
        public readonly IExpression Expr;
        public readonly string Alias;

        public FieldAliasExpression(IExpression expr, string alias)
        {
            Expr = expr;
            Alias = alias;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append("(");
            Expr.BuildQuery(sb);
            sb.Append(") AS ");
            sb.Append(Alias);
        }
    }

    public class SelectClause : IQueryPart
    {
        public readonly IEnumerable<ISelection> Sel;

        public SelectClause(IEnumerable<ISelection> sel)
        {
            Sel = sel;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append("SELECT ");
            QueryHelper.BuildJoinedExpression(sb, ", ", Sel);
        }
    }
}
