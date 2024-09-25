namespace LazurdIT.FluentOrm.Common;

public interface IUpdateExpression
{
    string GetExpression(string parameterName, string expressionSymbol);

    bool HasParameter { get; }
}