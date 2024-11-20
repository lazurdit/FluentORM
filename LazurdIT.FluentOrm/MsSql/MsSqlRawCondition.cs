using LazurdIT.FluentOrm.Common;
using System.Data.SqlClient;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlRawCondition : FluentRawCondition
    {
    }

    public class MsSqlRawCondition<T> : FluentSingleAttributeCondition<T, string>
    {
        public override bool HasParameters => false;

        public override SqlParameter[]? GetDbParameters() => null;

        public override string GetExpression() => $"({AttributeName} {Value})";
    }
}