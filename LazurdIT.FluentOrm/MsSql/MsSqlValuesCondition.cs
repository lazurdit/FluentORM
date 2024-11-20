using LazurdIT.FluentOrm.Common;
using System.Data.Common;
using System.Data.SqlClient;

namespace LazurdIT.FluentOrm.MsSql
{
    public abstract class MsSqlValuesCondition<T, TProperty> : FluentSingleAttributeCondition<T, TProperty>
    {
        public override DbParameter[]? GetDbParameters() => GetSqlParameters();

        public virtual SqlParameter[]? GetSqlParameters()
        {
            return new[] { new SqlParameter(ParameterName, Value) };
        }
    }
}