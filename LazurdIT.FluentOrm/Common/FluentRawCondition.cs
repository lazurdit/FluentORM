using System.Data.Common;

namespace LazurdIT.FluentOrm.Common
{
    public class FluentRawCondition : IFluentCondition
    {
        public string RawCondition { get; set; } = string.Empty;

        public bool HasParameters => false;

        public string ExpressionSymbol
        { get => string.Empty; set { } }

        public string ParameterName
        { get => string.Empty; set { } }

        public DbParameter[]? GetDbParameters() => null;

        public string GetExpression() => RawCondition;

        public IFluentCondition SetExpressionSymbol(string expressionSymbol) => this;

        public IFluentCondition SetParameterName(string parameterName) => this;
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