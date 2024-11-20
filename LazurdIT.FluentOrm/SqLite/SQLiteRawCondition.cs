using LazurdIT.FluentOrm.Common;
using System.Data.SQLite;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteRawCondition : FluentRawCondition
    {
    }

    public class SQLiteRawCondition<T> : FluentSingleAttributeCondition<T, string>
    {
        public override bool HasParameters => false;

        public override SQLiteParameter[]? GetDbParameters() => null;

        public override string GetExpression() => $"({AttributeName} {Value})";
    }
}