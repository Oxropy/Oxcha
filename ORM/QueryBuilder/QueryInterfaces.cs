using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORM.QueryBuilder
{
    public interface IQueryPart
    {
        void BuildQuery(StringBuilder sb);
    }

    public interface ISelection : IQueryPart { }

    public interface IExpression : IQueryPart { }

    public interface ITruthy : IExpression { }

    public interface ILiteralExpression : IExpression { }

    public interface IOrderBy : IQueryPart { }

    public interface ICreate : IExpression { }

    public interface IInsert : IExpression { }
}
