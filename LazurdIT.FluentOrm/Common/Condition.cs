using System.Data.Common;

namespace LazurdIT.FluentOrm.Common
{
    public abstract class Condition : ISingleAttributeCondition
    {
        public string ParameterName { get; set; } = "";

        //public abstract SqlParameter[]? GetSqlParameters();

        public ISingleAttributeCondition SetParameterName(string parameterName)
        {
            ParameterName = parameterName;
            return this;
        }

        public abstract bool HasParameters { get; }

        public abstract string GetExpression(string expressionSymbol);

        public abstract DbParameter[]? GetDbParameters(string expressionSymbol);

        public string AttributeName { get; set; } = string.Empty;
    }

    public abstract class Condition<T> : Condition, ICondition<T>, ISingleAttributeCondition
    {
        public T? Value { get; set; }
    }

    public abstract class ValuesCondition<T, TProperty> : ICondition<T, TProperty>, ISingleAttributeCondition
    {
        public string AttributeName { get; set; } = string.Empty;

        public TProperty? Value { get; set; }

        public string ParameterName { get; set; } = string.Empty;

        public ISingleAttributeCondition SetParameterName(string parameterName)
        {
            ParameterName = parameterName;
            return this;
        }

        public abstract bool HasParameters { get; }

        public abstract string GetExpression(string expressionSymbol);

        public abstract DbParameter[]? GetDbParameters(string expressionSymbol);
    }
}