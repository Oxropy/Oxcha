using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.QueryBuilder
{
    public enum SortOrder
    {
        Asc,
        Desc
    }

    public class SortOrderClause : IOrderBy
    {
        public readonly FieldReferenceExpression Field;
        public readonly SortOrder Sort;

        public SortOrderClause(FieldReferenceExpression field, SortOrder sort = SortOrder.Asc)
        {
            Field = field;
            Sort = sort;
        }

        public void BuildQuery(StringBuilder sb)
        {
            Field.BuildQuery(sb);
            sb.Append(Sort);
        }
    }

    public class OrderByClause : IQueryPart
    {
        public readonly IEnumerable<IOrderBy> OrderBy;

        public OrderByClause(IEnumerable<IOrderBy> orderBy)
        {
            OrderBy = orderBy;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append("ORDER BY ");
            QueryHelper.BuildJoinedExpression(sb, ", ", OrderBy);
        }
    }
}
