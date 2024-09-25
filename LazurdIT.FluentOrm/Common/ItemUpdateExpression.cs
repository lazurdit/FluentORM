namespace LazurdIT.FluentOrm.Common;

public class ItemUpdateExpression : IUpdateExpression
{
    public string GetExpression(string parameterName, string expressionSymbol)
    {
        return $"{AttributeName} = {expressionSymbol}{parameterName}{AttributeName}";
    }

    public string AttributeName { get; }

    public bool HasParameter => true;

    public ItemUpdateExpression(string attributeName)
    {
        AttributeName = attributeName;
    }
}