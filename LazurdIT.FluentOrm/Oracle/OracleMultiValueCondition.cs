using System.Data.Common;
using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;

namespace LazurdIT.FluentOrm.Oracle;

public abstract class OracleMultiValueCondition<T, TProperty> : IMultiValueCondition<T, TProperty>, ISingleAttributeCondition
{
    public string AttributeName { get; set; } = string.Empty;

    public string ParameterName { get; set; } = string.Empty;

    public TProperty[]? Values { get; set; }

    public OracleParameter[]? GetSqlParameters(string _)
    {
        return Values?.Select((value, index) => new OracleParameter($"{ParameterName}_{index}", value)).ToArray();
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