using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Common;

public class ItemOrderByExpression : IItemOrderByExpression
{
    public string Expression { get; }
    public bool IsRandom => false;
    public string AttributeName { get; }
    public OrderDirections Direction { get; }

    public ItemOrderByExpression(string attributeName, OrderDirections direction = OrderDirections.Ascending)
    {
        AttributeName = attributeName;
        Direction = direction;
        Expression = $"{AttributeName} {(Direction == OrderDirections.Descending ? "desc" : "")}";
    }

    public ItemOrderByExpression(string expression)
    {
        AttributeName = string.Empty;
        Direction = OrderDirections.Ascending;
        Expression = expression;
    }

    public static ItemOrderByExpression FromField<T, TProperty>(Expression<Func<T, TProperty>> property, OrderDirections direction = OrderDirections.Ascending) where T : IFluentModel, new()
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new ItemOrderByExpression(propertyOrField, direction);
    }
}