using System.Data.Common;

namespace LazurdIT.FluentOrm.Common
{
    /// <summary>
    /// Describes Base Fluent condition.
    /// </summary>
    public interface IFluentCondition
    {
        string GetExpression();

        bool HasParameters { get; }

        string ExpressionSymbol { get; set; }

        IFluentCondition SetExpressionSymbol(string expressionSymbol);

        DbParameter[]? GetDbParameters();

        string ParameterName { get; set; }

        IFluentCondition SetParameterName(string parameterName);
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