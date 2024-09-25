using System.Data.Common;

namespace LazurdIT.FluentOrm.Common;

public interface ICondition
{
    string GetExpression(string expressionSymbol);

    bool HasParameters { get; }

    DbParameter[]? GetDbParameters(string expressionSymbol);

    string ParameterName { get; set; }

    ISingleAttributeCondition SetParameterName(string parameterName);
}

public interface ISingleAttributeCondition : ICondition
{
    string AttributeName { get; set; }
}

public interface ICondition<T>
{
    T? Value { get; set; }
}

public interface ICondition<T, TProperty>
{
    TProperty? Value { get; set; }
}