using LazurdIT.FluentOrm.Common;
using System.Data.Common;
using System.Data.SQLite;

namespace LazurdIT.FluentOrm.SQLite
{
    public abstract class SQLiteValuesCondition<T, TProperty> : FluentSingleAttributeCondition<T, TProperty>
    {
        public override DbParameter[]? GetDbParameters() => GetSqlParameters();

        public virtual SQLiteParameter[]? GetSqlParameters()
        {
            return new[] { new SQLiteParameter(ParameterName, Value) };
        }
    }
}