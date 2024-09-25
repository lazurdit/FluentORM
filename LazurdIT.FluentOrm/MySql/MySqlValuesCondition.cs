using System.Data.Common;
using LazurdIT.FluentOrm.Common;
using MySqlConnector;

namespace LazurdIT.FluentOrm.MySql;

public abstract class MySqlValuesCondition<T, TProperty> : ValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
{
    public override DbParameter[]? GetDbParameters(string expressionSymbol) => GetSqlParameters(expressionSymbol);

    public virtual MySqlParameter[]? GetSqlParameters(string expressionSymbol)
    {
        return new[] { new MySqlParameter($"{expressionSymbol}{ParameterName}", Value) };
    }
}