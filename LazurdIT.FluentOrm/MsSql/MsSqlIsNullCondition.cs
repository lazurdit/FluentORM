using LazurdIT.FluentOrm.Common;
using System.Data.Common;
using System.Data.SqlClient;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlIsNullCondition : Condition<bool>
    {
        public override bool HasParameters => false;

        public override string GetExpression(string expressionSymbol)
        {
            return $"({AttributeName} is {(Value ? "" : " not ")} null)";
        }

        public SqlParameter[]? GetSqlParameters(string _) => null;

        public override DbParameter[]? GetDbParameters(string _) => null;
    }
}