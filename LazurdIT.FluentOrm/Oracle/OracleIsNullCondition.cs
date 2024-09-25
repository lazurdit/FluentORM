using System.Data.Common;
using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;

namespace LazurdIT.FluentOrm.Oracle;

public class OracleIsNullCondition : Condition<bool>
{
    public override bool HasParameters => false;

    public override string GetExpression(string expressionSymbol)
    {
        return $"({AttributeName} is {(Value ? "" : " not ")} null)";
    }

    public OracleParameter[]? GetSqlParameters(string _) => null;

    public override DbParameter[]? GetDbParameters(string expressionSymbol) => null;
}