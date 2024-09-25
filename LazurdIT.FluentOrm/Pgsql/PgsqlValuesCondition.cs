using LazurdIT.FluentOrm.Common;
using Npgsql;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Pgsql;

public abstract class PgsqlValuesCondition<T, TProperty> : ValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
{
    public override DbParameter[]? GetDbParameters(string expressionSymbol) => GetSqlParameters(expressionSymbol);

    public virtual NpgsqlParameter[]? GetSqlParameters(string expressionSymbol)
    {
        return new[] { new NpgsqlParameter(ParameterName, Value) };
    }
}