using LazurdIT.FluentOrm.Common;
using Npgsql;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlIsNullCondition : Condition<bool>
    {
        public override bool HasParameters => false;

        public override string GetExpression(string expressionSymbol)
        {
            return $"({AttributeName} is {(Value ? "" : " not ")} null)";
        }

        public NpgsqlParameter[]? GetPgsqlParameters(string expressionSymbol) => null;

        public override DbParameter[]? GetDbParameters(string expressionSymbol) => null;
    }
}