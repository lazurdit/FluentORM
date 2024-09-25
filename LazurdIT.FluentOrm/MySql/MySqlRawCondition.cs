using System.Data.Common;
using LazurdIT.FluentOrm.Common;
using MySqlConnector;

namespace LazurdIT.FluentOrm.MySql;

public class MySqlRawCondition : Condition<string>
{
    public override bool HasParameters => false;

    public override DbParameter[]? GetDbParameters(string expressionSymbol) => null;

    public override string GetExpression(string expressionSymbol) => $"{Value}";
}

public class MySqlRawCondition<T> : ValuesCondition<T, string>, ICondition<T, string>, ISingleAttributeCondition
{
    public override bool HasParameters => false;

    public override MySqlParameter[]? GetDbParameters(string expressionSymbol) => null;

    public override string GetExpression(string expressionSymbol) => $"({AttributeName} {Value})";
}