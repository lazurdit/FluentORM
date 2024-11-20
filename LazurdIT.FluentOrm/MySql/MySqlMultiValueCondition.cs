using LazurdIT.FluentOrm.Common;
using MySqlConnector;
using System.Data.Common;
using System.Linq;

namespace LazurdIT.FluentOrm.MySql
{
    public abstract class MySqlMultiValueCondition<T, TProperty> : IMultiValueSingleAttributeCondition<T, TProperty>
    {
        public string AttributeName { get; set; } = string.Empty;

        public string ParameterName { get; set; } = string.Empty;

        public TProperty[]? Values { get; set; }

        public MySqlParameter[]? GetSqlParameters()
        {
            return Values?.Select((value, index) => new MySqlParameter($"{ExpressionSymbol}{ParameterName}_{index}", value)).ToArray();
        }

        public abstract bool HasParameters { get; }

        public abstract string GetExpression();

        public DbParameter[]? GetDbParameters() => GetSqlParameters();

        public string ExpressionSymbol { get; set; } = string.Empty;

        public MySqlMultiValueCondition<T, TProperty> SetParameterName(string parameterName)
        {
            ParameterName = parameterName;
            return this;
        }

        IFluentCondition IFluentCondition.SetParameterName(string parameterName) => SetParameterName(parameterName);

        public MySqlMultiValueCondition<T, TProperty> SetExpressionSymbol(string expressionSymbol)
        {
            ExpressionSymbol = expressionSymbol;
            return this;
        }

        IFluentCondition IFluentCondition.SetExpressionSymbol(string parameterName) => SetExpressionSymbol(parameterName);
    }
}