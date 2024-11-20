using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;

namespace LazurdIT.FluentOrm.Oracle
{
    public class OracleRawCondition : FluentRawCondition
    {
    }

    public class OracleRawCondition<T> : FluentSingleAttributeCondition<T, string>
    {
        public override bool HasParameters => false;

        public override OracleParameter[]? GetDbParameters() => null;

        public override string GetExpression() => $"({AttributeName} {Value})";
    }
}