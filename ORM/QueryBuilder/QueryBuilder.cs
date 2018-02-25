using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.QueryBuilder
{
    public class FieldReferenceExpression : IExpression, ISelection
    {
        public readonly string TableName;
        public readonly string FieldName;

        public FieldReferenceExpression(string fieldName, string tableName = "")
        {
            TableName = tableName;
            FieldName = fieldName;
        }

        public void BuildQuery(StringBuilder sb)
        {
            if (!string.IsNullOrWhiteSpace(TableName))
            {
                sb.Append(TableName);
                sb.Append(".");
            }
            sb.Append(FieldName);
        }
    }

    public class FunctionCallExpression : IExpression, ISelection
    {
        public readonly string FunctionName;
        public readonly IEnumerable<IExpression> Parameters;

        public FunctionCallExpression(string functionName, IEnumerable<IExpression> parameters)
        {
            FunctionName = functionName;
            Parameters = parameters;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append(FunctionName);
            sb.Append("(");
            QueryHelper.BuildJoinedExpression(sb, ", ", Parameters);
            sb.Append(")");
        }
    }

    public static class QueryBuilderExtensions
    {
        public static string GetQuery(this IQueryPart part)
        {
            StringBuilder sb = new StringBuilder();
            part.BuildQuery(sb);
            return sb.ToString();
        }

        #region Select
        public static SelectClause Select(params ISelection[] s)
        {
            return new SelectClause(s);
        }

        public static ISelection As(this IExpression expr, string alias)
        {
            return new FieldAliasExpression(expr, alias);
        }
        #endregion

        #region From
        public static FromClause From(ITableName t)
        {
            return new FromClause(t);
        }

        public static ITableName Tbl(this string name, string alias = "")
        {
            return new TableName(name, alias);
        }

        public static ITableName Inr(this ITableName lhs, ITableName rhs, ITruthy comp)
        {
            return new JoinCondition(lhs, JoinClause.Inner, rhs, comp);
        }

        public static ITableName Lft(this ITableName lhs, ITableName rhs, ITruthy comp)
        {
            return new JoinCondition(lhs, JoinClause.Left, rhs, comp);
        }

        public static ITableName Rht(this ITableName lhs, ITableName rhs, ITruthy comp)
        {
            return new JoinCondition(lhs, JoinClause.Right, rhs, comp);
        }

        public static ITableName Fll(this ITableName lhs, ITableName rhs, ITruthy comp)
        {
            return new JoinCondition(lhs, JoinClause.Full, rhs, comp);
        }

        public static ITableName LftO(this ITableName lhs, ITableName rhs, ITruthy comp)
        {
            return new JoinCondition(lhs, JoinClause.LeftOuter, rhs, comp);
        }

        public static ITableName RhtO(this ITableName lhs, ITableName rhs, ITruthy comp)
        {
            return new JoinCondition(lhs, JoinClause.RightOuter, rhs, comp);
        }

        public static ITableName FllO(this ITableName lhs, ITableName rhs, ITruthy comp)
        {
            return new JoinCondition(lhs, JoinClause.FullOuter, rhs, comp);
        }

        public static ITableName Join(this ITableName lhs, JoinClause jn, ITableName rhs, ITruthy comp)
        {
            return new JoinCondition(lhs, jn, rhs, comp);
        }
        #endregion

        #region Where
        public static WhereClause Where(ITruthy t)
        {
            return new WhereClause(t);
        }

        public static ITruthy And(this ITruthy lhs, ITruthy rhs)
        {
            return Junc(lhs, JunctionOp.And, rhs);
        }

        public static ITruthy Or(this ITruthy lhs, ITruthy rhs)
        {
            return Junc(lhs, JunctionOp.Or, rhs);
        }

        public static ITruthy Junc(this ITruthy lhs, JunctionOp op, ITruthy rhs)
        {
            return new Junction(lhs, op, rhs);
        }

        public static ITruthy Eq(this IExpression lhs, IExpression rhs)
        {
            return Comp(lhs, ComparisonOperator.Equal, rhs);
        }

        public static ITruthy Neq(this IExpression lhs, IExpression rhs)
        {
            return Comp(lhs, ComparisonOperator.NotEqual, rhs);
        }

        public static ITruthy Gt(this IExpression lhs, IExpression rhs)
        {
            return Comp(lhs, ComparisonOperator.GreaterThan, rhs);
        }

        public static ITruthy GtEq(this IExpression lhs, IExpression rhs)
        {
            return Comp(lhs, ComparisonOperator.GreaterThanOrEqual, rhs);
        }

        public static ITruthy Lt(this IExpression lhs, IExpression rhs)
        {
            return Comp(lhs, ComparisonOperator.LowerThan, rhs);
        }

        public static ITruthy LtEq(this IExpression lhs, IExpression rhs)
        {
            return Comp(lhs, ComparisonOperator.LowerThanOrEqual, rhs);
        }

        public static ITruthy Comp(this IExpression lhs, ComparisonOperator co, IExpression rhs)
        {
            return new ComparisonExpression(lhs, co, rhs);
        }

        public static LiteralExpression Val(object s)
        {
            return new LiteralExpression(s);
        }

        public static FieldReferenceExpression Col(this string field, string table = "")
        {
            return new FieldReferenceExpression(field, table);
        }

        public static FunctionCallExpression Call(this string name, params IExpression[] parameters)
        {
            return new FunctionCallExpression(name, parameters);
        }

        public static ListExpression List(params IExpression[] parameters)
        {
            return new ListExpression(parameters);
        }

        public static ITruthy IsNull(this IExpression e)
        {
            return new IsNullExpression(e);
        }

        public static ITruthy In(this IExpression lhs, IExpression rhs)
        {
            return new InExpression(lhs, rhs);
        }

        public static ITruthy Like(this IExpression lhs, IExpression rhs)
        {
            return new LikeExpression(lhs, rhs);
        }

        public static PlaceholderExpression Plc()
        {
            return new PlaceholderExpression();
        }
        #endregion
    }

    public static class QueryHelper
    {
        public static void BuildJoinedExpression(StringBuilder sb, string seperator, IEnumerable<IQueryPart> part)
        {
            var e = part.GetEnumerator();
            if (e.MoveNext())
            {
                var v = e.Current;
                v.BuildQuery(sb);

                while (e.MoveNext())
                {
                    v = e.Current;
                    sb.Append(seperator);
                    v.BuildQuery(sb);
                }
            }
        }
    }
}
