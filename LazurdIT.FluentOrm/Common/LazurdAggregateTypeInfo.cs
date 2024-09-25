using System.Reflection;

namespace LazurdIT.FluentOrm.Common;

public class FluentAggregateTypeInfo
{
    private readonly PropertyInfo property;
    private readonly FluentFieldAttribute attribute;
    private AggregationMethods aggregationMethod;

    public FluentAggregateTypeInfo(PropertyInfo property, FluentFieldAttribute attribute, AggregationMethods aggregationMethod = AggregationMethods.Count, string? alias = null, string? customAggregationMethod = null)
    {
        this.property = property ?? throw new ArgumentNullException(nameof(property));
        this.attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
        this.aggregationMethod = aggregationMethod;
        this.Alias = alias;
        this.CustomAggregationMethod = customAggregationMethod ?? string.Empty;
    }

    public PropertyInfo Property => property;
    public FluentFieldAttribute Attribute => attribute;
    public AggregationMethods AggregationMethod { get => aggregationMethod; set => aggregationMethod = value; }
    public string FinalPropertyName => Attribute.Name ?? Property.Name;
    public string FinalAlias => string.IsNullOrEmpty(Alias) ? $"{FinalPropertyName}_{Enum.GetName(AggregationMethod)}" : $"{Alias} ";
    public string? Alias { get; set; }
    public string? CustomAggregationMethod { get; set; }

    public string GetExpressionOnly()
    {
        return $"{(aggregationMethod == AggregationMethods.Custom ? CustomAggregationMethod : Enum.GetName(aggregationMethod))}({FinalPropertyName})";
    }

    public string GetHeaderExpression()
    {
        if (aggregationMethod == AggregationMethods.Custom && string.IsNullOrEmpty(CustomAggregationMethod))
            throw new ArgumentException("Custom aggregation method must be provided when aggregation method is set to custom.");

        return $"{(aggregationMethod == AggregationMethods.Custom ? CustomAggregationMethod : Enum.GetName(AggregationMethod))}({FinalPropertyName} ) as {FinalAlias}";
    }
}