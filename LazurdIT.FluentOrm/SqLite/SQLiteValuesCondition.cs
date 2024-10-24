using LazurdIT.FluentOrm.Common;
using System.Data.Common;
using System.Data.SQLite;

namespace LazurdIT.FluentOrm.SQLite
{
    public abstract class SQLiteValuesCondition<T, TProperty> : ValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
    {
        public override DbParameter[]? GetDbParameters(string expressionSymbol) => GetSqlParameters(expressionSymbol);

        public virtual SQLiteParameter[]? GetSqlParameters(string expressionSymbol)
        {
            return new[] { new SQLiteParameter(ParameterName, Value) };
        }
    }
}