using LazurdIT.FluentOrm.Common;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteRawCondition : Condition<string>
    {
        public override bool HasParameters => false;

        public override DbParameter[]? GetDbParameters(string expressionSymbol) => null;

        public override string GetExpression(string expressionSymbol) => $"{Value}";
    }

    public class SQLiteRawCondition<T> : ValuesCondition<T, string>, ICondition<T, string>, ISingleAttributeCondition
    {
        public override bool HasParameters => false;

        public override SQLiteParameter[]? GetDbParameters(string expressionSymbol) => null;

        public override string GetExpression(string expressionSymbol) => $"({AttributeName} {Value})";
    }
}