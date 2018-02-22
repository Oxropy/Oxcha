using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.DataBase
{
    public interface IQueryPart
    {
        void BuildQuery(StringBuilder sb);
    }

    public interface IExpression : IQueryPart { }

    public interface ITruthy : IExpression { }

    public interface ILiteralExpression : IExpression { }

    public enum JunctionOp
    {
        And,
        Or
    }

    public enum ComparisonOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LowerThan,
        LowerThanOrEqual
    }

    public class Junction : ITruthy
    {
        public readonly ITruthy Lhs;
        public readonly JunctionOp Op;
        public readonly ITruthy Rhs;

        public Junction(ITruthy lhs, JunctionOp op, ITruthy rhs)
        {
            Lhs = lhs;
            Op = op;
            Rhs = rhs;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append("(");
            Lhs.BuildQuery(sb);
            sb.Append(") ");
            sb.Append(Op);
            sb.Append(" (");
            Rhs.BuildQuery(sb);
            sb.Append(")");
        }
    }

    public class LiteralExpression : ILiteralExpression
    {
        public readonly object Literal;

        public LiteralExpression(object literal)
        {
            Literal = literal;
        }

        public static string Sanitize(string s)
        {
            return s; // TODO: ???
        }

        public void BuildQuery(StringBuilder sb)
        {
            if (Literal is string)
            {
                sb.Append("'");
                sb.Append(Sanitize((string)Literal));
                sb.Append("'");
            }
            else
            {
                sb.Append(Literal);
            }
        }
    }

    public class FieldReferenceExpression : IExpression
    {
        public readonly string TableName;
        public readonly string FieldName;

        public FieldReferenceExpression(string tableName, string fieldName)
        {
            TableName = tableName;
            FieldName = fieldName;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append(TableName);
            sb.Append(".");
            sb.Append(FieldName);
        }
    }

    public class FunctionCallExpression : IExpression
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

    public class ListExpression : IExpression
    {
        public readonly IEnumerable<IExpression> Expressions;

        public ListExpression(IEnumerable<IExpression> expressions)
        {
            Expressions = expressions;
        }

        public void BuildQuery(StringBuilder sb)
        {
            QueryHelper.BuildJoinedExpression(sb, ", ", Expressions);
        }
    }

    public class IsNullExpression : ITruthy
    {
        public readonly IExpression Expr;

        public IsNullExpression(IExpression expr)
        {
            Expr = expr;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append("(");
            Expr.BuildQuery(sb);
            sb.Append(") IS Null");
        }
    }

    public class PlaceholderExpression : IExpression
    {
        public void BuildQuery(StringBuilder sb)
        {
            sb.Append("?");
        }
    }

    public class InExpression : ITruthy
    {
        public readonly IExpression Lhr;
        public readonly IExpression Rhr;

        public InExpression(IExpression lhr, IExpression rhr)
        {
            Lhr = lhr;
            Rhr = rhr;
        }

        public void BuildQuery(StringBuilder sb)
        {
            Lhr.BuildQuery(sb);
            sb.Append(" IN ");
            sb.Append("(");
            Rhr.BuildQuery(sb);
            sb.Append(")");
        }
    }

    public class LikeExpression : ITruthy
    {
        public readonly IExpression Lhr;
        public readonly IExpression Rhs;

        public LikeExpression(IExpression lhr, IExpression rhs)
        {
            Lhr = lhr;
            Rhs = rhs;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append(Lhr);
            sb.Append(" LIKE ");
            if (Rhs is LiteralExpression rhs)
            {
                if (rhs.Literal is string)
                {
                    Rhs.BuildQuery(sb);
                }
                else
                {
                    sb.Append("'");
                    Rhs.BuildQuery(sb);
                    sb.Append("'");
                }
            }
            else
            {
                Rhs.BuildQuery(sb);
            }
        }
    }

    public class ComparisonExpression : ITruthy
    {
        public readonly IExpression Lhs;
        public readonly ComparisonOperator Op;
        public readonly IExpression Rhs;

        public ComparisonExpression(IExpression lhs, ComparisonOperator op, IExpression rhs)
        {
            Lhs = lhs;
            Op = op;
            Rhs = rhs;
        }

        public void BuildQuery(StringBuilder sb)
        {
            Lhs.BuildQuery(sb);
            sb.Append(" ");
            sb.Append(GetOperatorValue(Op));
            sb.Append(" ");
            Rhs.BuildQuery(sb);
        }

        public static string GetOperatorValue(ComparisonOperator op)
        {
            switch (op)
            {
                case ComparisonOperator.Equal:
                    return "=";
                case ComparisonOperator.NotEqual:
                    return "!=";
                case ComparisonOperator.GreaterThan:
                    return ">";
                case ComparisonOperator.GreaterThanOrEqual:
                    return ">=";
                case ComparisonOperator.LowerThan:
                    return "<";
                case ComparisonOperator.LowerThanOrEqual:
                    return "<=";
                default:
                    throw new Exception("Operator unknown!");
            }
        }
    }

    public class WhereClause : IQueryPart
    {
        public readonly ITruthy Expr;

        public WhereClause(ITruthy expr)
        {
            Expr = expr;
        }

        public void BuildQuery(StringBuilder sb)
        {
            sb.Append("WHERE ");
            Expr.BuildQuery(sb);
        }
    }

    public static class QueryBuilderExtensions
    {
        public static string BuildQuery(this IQueryPart part)
        {
            StringBuilder sb = new StringBuilder();
            part.BuildQuery(sb);
            return sb.ToString();
        }

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

        public static FieldReferenceExpression Col(this string table, string field)
        {
            return new FieldReferenceExpression(table, field);
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
    }

    public static class QueryHelper
    {
        public static void BuildJoinedExpression(StringBuilder sb, string seperator, IEnumerable<IExpression> expression)
        {
            var e = expression.GetEnumerator();
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
