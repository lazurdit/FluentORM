using LazurdIT.FluentOrm.Common;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;

namespace LazurdIT.FluentOrm.SQLite
{
    public abstract class SQLiteMultiValueCondition<T, TProperty> : IMultiValueSingleAttributeCondition<T, TProperty>
    {
        public string AttributeName { get; set; } = string.Empty;

        public string ParameterName { get; set; } = string.Empty;

        public TProperty[]? Values { get; set; }

        public SQLiteParameter[]? GetSqlParameters()
        {
            return Values?.Select((value, index) => new SQLiteParameter($"{ExpressionSymbol}{ParameterName}_{index}", value)).ToArray();
        }

        public abstract bool HasParameters { get; }

        public abstract string GetExpression();

        public DbParameter[]? GetDbParameters() => GetSqlParameters();

        public string ExpressionSymbol { get; set; } = string.Empty;

        public SQLiteMultiValueCondition<T, TProperty> SetParameterName(string parameterName)
        {
            ParameterName = parameterName;
            return this;
        }

        IFluentCondition IFluentCondition.SetParameterName(string parameterName) => SetParameterName(parameterName);

        public SQLiteMultiValueCondition<T, TProperty> SetExpressionSymbol(string expressionSymbol)
        {
            ExpressionSymbol = expressionSymbol;
            return this;
        }

        IFluentCondition IFluentCondition.SetExpressionSymbol(string parameterName) => SetExpressionSymbol(parameterName);
    }
}