using LazurdIT.FluentOrm.Common;
using Npgsql;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Pgsql
{
    public abstract class PgsqlValuesCondition<T, TProperty> : FluentSingleAttributeCondition<T, TProperty>
    {
        public override DbParameter[]? GetDbParameters() => GetSqlParameters();

        public virtual NpgsqlParameter[]? GetSqlParameters()
        {
            return new[] { new NpgsqlParameter(ParameterName, Value) };
        }
    }
}