using System.Data.Common;

namespace LazurdIT.FluentOrm.Common
{
    public abstract class FluentSingleAttributeCondition<T> : IFluentSingleAttributeCondition<T>
    {
        public T? Value { get; set; }
        public string ParameterName { get; set; } = "";

        public FluentSingleAttributeCondition<T> SetParameterName(string parameterName)
        {
            ParameterName = parameterName;
            return this;
        }

        public abstract bool HasParameters { get; }

        public abstract string GetExpression();

        public abstract DbParameter[]? GetDbParameters();

        IFluentCondition IFluentCondition.SetParameterName(string parameterName) => SetParameterName(parameterName);

        public FluentSingleAttributeCondition<T> SetExpressionSymbol(string expressionSymbol)
        {
            ExpressionSymbol = expressionSymbol;
            return this;
        }

        IFluentCondition IFluentCondition.SetExpressionSymbol(string expressionSymbol) => SetExpressionSymbol(expressionSymbol);

        public string AttributeName { get; set; } = string.Empty;
        public string ExpressionSymbol { get; set; } = string.Empty;
    }

    public abstract class FluentSingleAttributeCondition<T, TProperty> : IFluentSingleAttributeCondition<T, TProperty>
    {
        public string AttributeName { get; set; } = string.Empty;

        public TProperty? Value { get; set; }

        public string ParameterName { get; set; } = string.Empty;

        public FluentSingleAttributeCondition<T, TProperty> SetParameterName(string parameterName)
        {
            ParameterName = parameterName;
            return this;
        }

        public abstract bool HasParameters { get; }
        public string ExpressionSymbol { get; set; } = string.Empty;

        public abstract string GetExpression();

        public abstract DbParameter[]? GetDbParameters();

        IFluentCondition IFluentCondition.SetParameterName(string parameterName) => SetParameterName(parameterName);

        public FluentSingleAttributeCondition<T, TProperty> SetExpressionSymbol(string expressionSymbol)
        {
            ExpressionSymbol = expressionSymbol;
            return this;
        }

        IFluentCondition IFluentCondition.SetExpressionSymbol(string expressionSymbol) => SetExpressionSymbol(expressionSymbol);
    }
}