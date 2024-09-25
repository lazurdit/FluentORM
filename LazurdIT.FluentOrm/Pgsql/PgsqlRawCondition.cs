using LazurdIT.FluentOrm.Common;
using Npgsql;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Pgsql;

public class PgsqlRawCondition : Condition<string>
{
    public override bool HasParameters => false;

    public override DbParameter[]? GetDbParameters(string expressionSymbol) => null;

    public override string GetExpression(string expressionSymbol) => $"{Value}";
}

public class PgsqlRawCondition<T> : ValuesCondition<T, string>, ICondition<T, string>, ISingleAttributeCondition
{
    public override bool HasParameters => false;

    public override NpgsqlParameter[]? GetDbParameters(string expressionSymbol) => null;

    public override string GetExpression(string symb) => $"({AttributeName} {Value})";
}