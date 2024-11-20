using LazurdIT.FluentOrm.Common;
using System.Data.Common;
using System.Data.SqlClient;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlIsNullCondition : FluentSingleAttributeCondition<bool>
    {
        public override bool HasParameters => false;

        public override string GetExpression() => $"({AttributeName} is {(Value ? "" : " not ")} null)";

        public static SqlParameter[]? GetSqlParameters() => null;

        public override DbParameter[]? GetDbParameters() => null;
    }
}