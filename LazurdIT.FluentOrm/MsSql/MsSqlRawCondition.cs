using LazurdIT.FluentOrm.Common;
using System.Data.Common;
using System.Data.SqlClient;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlRawCondition : Condition<string>
    {
        public override bool HasParameters => false;

        public override DbParameter[]? GetDbParameters(string expressionSymbol) => null;

        public override string GetExpression(string expressionSymbol) => $"{Value}";
    }

    public class MsSqlRawCondition<T> : ValuesCondition<T, string>, ICondition<T, string>, ISingleAttributeCondition
    {
        public override bool HasParameters => false;

        public override SqlParameter[]? GetDbParameters(string expressionSymbol) => null;

        public override string GetExpression(string expressionSymbol) => $"({AttributeName} {Value})";
    }
}