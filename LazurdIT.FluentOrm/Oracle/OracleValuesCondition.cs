using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Oracle
{
    public abstract class OracleValuesCondition<T, TProperty> : FluentSingleAttributeCondition<T, TProperty>
    {
        public override DbParameter[]? GetDbParameters() => GetSqlParameters();

        public virtual OracleParameter[]? GetSqlParameters()
        {
            return new[] { new OracleParameter($"{ParameterName}", Value) };
        }
    }
}