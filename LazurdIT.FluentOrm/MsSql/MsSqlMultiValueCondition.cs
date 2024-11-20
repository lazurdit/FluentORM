using LazurdIT.FluentOrm.Common;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace LazurdIT.FluentOrm.MsSql
{
    public abstract class MsSqlMultiValueCondition<T, TProperty> : IMultiValueSingleAttributeCondition<T, TProperty>
    {
        public string AttributeName { get; set; } = string.Empty;

        public string ParameterName { get; set; } = string.Empty;

        public TProperty[]? Values { get; set; }

        public SqlParameter[]? GetSqlParameters()
        {
            return Values?.Select((value, index) => new SqlParameter($"{ExpressionSymbol}{ParameterName}_{index}", value)).ToArray();
        }

        public abstract bool HasParameters { get; }
        public string ExpressionSymbol { get; set; } = string.Empty;

        public abstract string GetExpression();

        public DbParameter[]? GetDbParameters() => GetSqlParameters();

        public MsSqlMultiValueCondition<T, TProperty> SetParameterName(string parameterName)
        {
            ParameterName = parameterName;
            return this;
        }

        IFluentCondition IFluentCondition.SetParameterName(string parameterName) => SetParameterName(parameterName);

        public MsSqlMultiValueCondition<T, TProperty> SetExpressionSymbol(string expressionSymbol)
        {
            ExpressionSymbol = expressionSymbol;
            return this;
        }

        IFluentCondition IFluentCondition.SetExpressionSymbol(string parameterName) => SetExpressionSymbol(parameterName);
    }
}