using LazurdIT.FluentOrm.Common;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteIsNullCondition : Condition<bool>
    {
        public override bool HasParameters => false;

        public override string GetExpression(string expressionSymbol)
        {
            return $"({AttributeName} is {(Value ? "" : " not ")} null)";
        }

        public SQLiteParameter[]? GetSqlParameters(string _) => null;

        public override DbParameter[]? GetDbParameters(string _) => null;
    }
}