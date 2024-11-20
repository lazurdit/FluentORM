using System.Data.Common;

namespace LazurdIT.FluentOrm.Common
{
    public abstract class FluentCondition : IFluentSingleAttributeCondition
    {
        public string ParameterName { get; set; } = "";

        public abstract bool HasParameters { get; }

        public abstract string GetExpression();

        public abstract DbParameter[]? GetDbParameters();

        public FluentCondition SetParameterName(string parameterName)
        {
            ParameterName = parameterName;
            return this;
        }

        IFluentCondition IFluentCondition.SetParameterName(string parameterName) => SetParameterName(parameterName);

        public FluentCondition SetExpressionSymbol(string expressionSymbol)
        {
            ExpressionSymbol = expressionSymbol;
            return this;
        }

        IFluentCondition IFluentCondition.SetExpressionSymbol(string expressionSymbol) => SetExpressionSymbol(expressionSymbol);

        public string AttributeName { get; set; } = string.Empty;
        public string ExpressionSymbol { get; set; } = string.Empty;
    }

    //public interface IFluentCondition
    //{
    //    string GetExpression(string expressionSymbol);

    //    bool HasParameters { get; }

    //    DbParameter[]? GetDbParameters(string expressionSymbol);

    //    string ParameterName { get; set; }

    //    ISingleAttributeCondition SetParameterName(string parameterName);
    //}

    //public interface ISingleAttributeCondition : IFluentCondition
    //{
    //    string AttributeName { get; set; }
    //}

    //public interface IFluentCondition<T>
    //{
    //    T? Value { get; set; }
    //}

    //public interface IFluentCondition<T, TProperty>
    //{
    //    TProperty? Value { get; set; }
    //}
}