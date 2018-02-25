using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.QueryBuilder
{
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
    
}
