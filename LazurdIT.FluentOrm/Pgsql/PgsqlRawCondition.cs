using LazurdIT.FluentOrm.Common;
using Npgsql;

namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlRawCondition : FluentRawCondition
    {
    }

    public class PgsqlRawCondition<T> : FluentSingleAttributeCondition<T, string>
    {
        public override bool HasParameters => false;

        public override NpgsqlParameter[]? GetDbParameters() => null;

        public override string GetExpression() => $"({AttributeName} {Value})";
    }
}