using LazurdIT.FluentOrm.Common;
using MySqlConnector;
using System.Data.Common;

namespace LazurdIT.FluentOrm.MySql
{
    public abstract class MySqlValuesCondition<T, TProperty> : FluentSingleAttributeCondition<T, TProperty>
    {
        public override DbParameter[]? GetDbParameters() => GetSqlParameters();

        public virtual MySqlParameter[]? GetSqlParameters()
        {
            return new[] { new MySqlParameter($"{ExpressionSymbol}{ParameterName}", Value) };
        }
    }
}