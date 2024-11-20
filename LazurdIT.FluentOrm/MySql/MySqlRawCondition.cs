using LazurdIT.FluentOrm.Common;
using MySqlConnector;

namespace LazurdIT.FluentOrm.MySql
{
    public class MySqlRawCondition : FluentRawCondition
    {
    }

    public class MySqlRawCondition<T> : FluentSingleAttributeCondition<T, string>
    {
        public override bool HasParameters => false;

        public override MySqlParameter[]? GetDbParameters() => null;

        public override string GetExpression() => $"({AttributeName} {Value})";
    }
}