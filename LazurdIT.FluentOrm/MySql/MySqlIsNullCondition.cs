using LazurdIT.FluentOrm.Common;
using MySqlConnector;
using System.Data.Common;

namespace LazurdIT.FluentOrm.MySql
{
    public class MySqlIsNullCondition : Condition<bool>
    {
        public override bool HasParameters => false;

        public override string GetExpression(string expressionSymbol)
        {
            return $"({AttributeName} is {(Value ? "" : " not ")} null)";
        }

        public MySqlParameter[]? GetSqlParameters(string expressionSymbol) => null;

        public override DbParameter[]? GetDbParameters(string expressionSymbol) => null;
    }
}