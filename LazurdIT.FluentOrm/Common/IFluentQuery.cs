using System.Data;

namespace LazurdIT.FluentOrm.Common
{
    public interface IFluentQuery
    {
        string ExpressionSymbol { get; }

        IDbConnection? Connection { get; set; }

        IFluentQuery WithConnection(IDbConnection? connection);
    }
}