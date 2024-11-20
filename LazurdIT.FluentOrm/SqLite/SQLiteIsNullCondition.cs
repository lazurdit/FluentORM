using LazurdIT.FluentOrm.Common;
using System.Data.Common;
using System.Data.SqlClient;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteIsNullCondition : FluentSingleAttributeCondition<bool>
    {
        public override bool HasParameters => false;

        public override string GetExpression()
        {
            return $"({AttributeName} is {(Value ? "" : " not ")} null)";
        }

        public static SqlParameter[]? GetSqlParameters() => null;

        public override DbParameter[]? GetDbParameters() => null;
    }
}