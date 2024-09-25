using System.Data.Common;
using System.Data.SqlClient;
using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.MsSql;

public abstract class MsSqlValuesCondition<T, TProperty> : ValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
{
    public override DbParameter[]? GetDbParameters(string expressionSymbol) => GetSqlParameters(expressionSymbol);

    public virtual SqlParameter[]? GetSqlParameters(string expressionSymbol)
    {
        return new[] { new SqlParameter(ParameterName, Value) };
    }
}