using System.Data.Common;
using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;

namespace LazurdIT.FluentOrm.Oracle;

public class OracleRawCondition : Condition<string>
{
    public override bool HasParameters => false;

    public override DbParameter[]? GetDbParameters(string expressionSymbol) => null;

    public override string GetExpression(string expressionSymbol) => $"{Value}";
}

public class OracleRawCondition<T> : ValuesCondition<T, string>, ICondition<T, string>, ISingleAttributeCondition
{
    public override bool HasParameters => false;

    public override OracleParameter[]? GetDbParameters(string expressionSymbol) => null;

    public override string GetExpression(string expressionSymbol) => $"({AttributeName} {Value})";
}