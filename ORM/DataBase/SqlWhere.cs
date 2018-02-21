using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.DataBase
{
    public interface IExpression { }

    public interface ITruthy : IExpression { }

    public interface ILikeCompatible : IExpression { }

    public interface ILiteralExpression : IExpression { }

    public enum JunctionOp
    {
        And,
        Or
    }

    public enum SqlComparisonOperator
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

        public override string ToString()
        {
            return string.Format("({0}) {1} ({2})", Lhs, Op, Rhs);
        }
    }

    public class LiteralExpression : ILiteralExpression, ILikeCompatible
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

        public override string ToString()
        {
            if (Literal is string)
            {
                return string.Format(@"'{0}'", Sanitize((string)Literal));
            }

            return Literal.ToString();
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

        public override string ToString()
        {
            return string.Format("{0}.{1}", TableName, FieldName);
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

        public override string ToString()
        {
            return string.Format("{0}({1})", FunctionName, string.Join(" , ", Parameters));
        }
    }

    public class ListExpression : IExpression
    {
        public readonly IEnumerable<IExpression> Expressions;

        public ListExpression(IEnumerable<IExpression> expressions)
        {
            Expressions = expressions;
        }

        public override string ToString()
        {
            if (Expressions == null)
            {
                return string.Empty;
            }

            return string.Format("({0})", string.Join(" , ", Expressions));
        }
    }

    public class IsNullExpression : ITruthy
    {
        public readonly IExpression Expr;

        public IsNullExpression(IExpression expr)
        {
            Expr = expr;
        }

        public override string ToString()
        {
            return string.Format("({0}) IS Null", Expr);
        }
    }

    public class PlaceholderExpression : IExpression
    {
        public override string ToString()
        {
            return "?";
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

        public override string ToString()
        {
            return string.Format("({0}) IN ({1})", Lhr, Rhr);
        }
    }

    public class LikeExpression : ITruthy
    {
        public readonly IExpression Lhr;
        public readonly ILikeCompatible Rhs;

        public LikeExpression(IExpression lhr, ILikeCompatible rhs)
        {
            Lhr = lhr;
            Rhs = rhs;
        }

        public override string ToString()
        {
            return string.Format("({0}) LIKE ({1})", Lhr, Rhs);
        }
    }

    public class ComparisonExpression : ITruthy
    {
        public readonly IExpression Lhs;
        public readonly SqlComparisonOperator Op;
        public readonly IExpression Rhs;

        public ComparisonExpression(IExpression lhs, SqlComparisonOperator op, IExpression rhs)
        {
            Lhs = lhs;
            Op = op;
            Rhs = rhs;
        }

        public override string ToString()
        {
            string op;
            switch (Op)
            {
                case SqlComparisonOperator.Equal:
                    op = "=";
                    break;
                case SqlComparisonOperator.NotEqual:
                    op = "!=";
                    break;
                case SqlComparisonOperator.GreaterThan:
                    op = ">";
                    break;
                case SqlComparisonOperator.GreaterThanOrEqual:
                    op = ">=";
                    break;
                case SqlComparisonOperator.LowerThan:
                    op = "<";
                    break;
                case SqlComparisonOperator.LowerThanOrEqual:
                    op = "<=";
                    break;
                default:
                    throw new Exception();
            }

            return string.Format("{0} {1} {2}", Lhs, op, Rhs);
        }
    }
    
    public class WhereClause
    {
        public readonly ITruthy Expr;

        public WhereClause(ITruthy expr)
        {
            Expr = expr;
        }

        public override string ToString()
        {
            return string.Format("WHERE {0}", Expr);
        }
    }

    public static class Q
    {
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
            return Comp(lhs, SqlComparisonOperator.Equal, rhs);
        }

        public static ITruthy Neq(this IExpression lhs,IExpression rhs)
        {
            return Comp(lhs, SqlComparisonOperator.NotEqual, rhs);
        }

        public static ITruthy Gt(this IExpression lhs, IExpression rhs)
        {
            return Comp(lhs, SqlComparisonOperator.GreaterThan, rhs);
        }

        public static ITruthy GtEq(this IExpression lhs, IExpression rhs)
        {
            return Comp(lhs, SqlComparisonOperator.GreaterThanOrEqual, rhs);
        }

        public static ITruthy Lt(this IExpression lhs, IExpression rhs)
        {
            return Comp(lhs, SqlComparisonOperator.LowerThan, rhs);
        }

        public static ITruthy LtEq(this IExpression lhs, IExpression rhs)
        {
            return Comp(lhs, SqlComparisonOperator.LowerThanOrEqual, rhs);
        }

        public static ITruthy Comp(this IExpression lhs, SqlComparisonOperator co, IExpression rhs)
        {
            return new ComparisonExpression(lhs, co, rhs);
        }

        public static LiteralExpression Val(this object s)
        {
            return new LiteralExpression(s);
        }

        public static FieldReferenceExpression Col(this string table, string field)
        {
            return new FieldReferenceExpression(table, field);
        }

        public static FunctionCallExpression Fn(this string name, params IExpression[] parameters)
        {
            return new FunctionCallExpression(name, parameters);
        }

        public static ListExpression Lst(params IExpression[] parameters)
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

        public static ITruthy Lke(this IExpression lhs, ILikeCompatible rhs)
        {
            return new LikeExpression(lhs, rhs);
        }

        public static PlaceholderExpression Plc()
        {
            return new PlaceholderExpression();
        }
    }
    
}
