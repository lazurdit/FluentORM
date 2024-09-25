using System.Data.Common;
using System.Data.SqlClient;
using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.MsSql;

public abstract class MsSqlMultiValueCondition<T, TProperty> : IMultiValueCondition<T, TProperty>, ISingleAttributeCondition
{
    public string AttributeName { get; set; } = string.Empty;

    public string ParameterName { get; set; } = string.Empty;

    public TProperty[]? Values { get; set; }

    public SqlParameter[]? GetSqlParameters(string expressionSymbol)
    {
        return Values?.Select((value, index) => new SqlParameter($"{expressionSymbol}{ParameterName}_{index}", value)).ToArray();
    }

    public ISingleAttributeCondition SetParameterName(string parameterName)
    {
        ParameterName = parameterName;
        return this;
    }

    public abstract bool HasParameters { get; }

    public abstract string GetExpression(string expressionSymbol);

    public DbParameter[]? GetDbParameters(string expressionSymbol) => GetSqlParameters(expressionSymbol);
}