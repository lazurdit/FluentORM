using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Common;

public class FluentRelation<T1, T2> : IFluentRelation
    where T1 : IFluentModel, new()
    where T2 : IFluentModel, new()
{
    public string SourceTableName { get; }
    public string TargetTableName { get; }
    public string? SourceTablePrefix { get; } = null;
    public string? TargetTablePrefix { get; } = null;

    protected readonly FluentTypeDictionary sourceOriginalFields;
    protected readonly FluentTypeDictionary targetOriginalFields;
    public List<RelationFields> Fields { get; }

    public virtual string RelationName { get; set; }

    public FluentRelation(string? relationName = null, string? sourceTablePrefix = null, string? targetTablePrefix = null)
    {
        Fields = new();
        sourceOriginalFields = new(TypeCache.GetTypeCache<T1>());
        targetOriginalFields = new(TypeCache.GetTypeCache<T2>());
        RelationName = relationName ?? $"{typeof(T1).Name}_{typeof(T2).Name}";
        SourceTableName = AttributeResolver.ResolveTableName<T1>();
        TargetTableName = AttributeResolver.ResolveTableName<T2>();
        SourceTablePrefix = sourceTablePrefix;
        TargetTablePrefix = targetTablePrefix;
    }

    public FluentRelation<T1, T2> WithField<TProperty>(
        Expression<Func<T1, TProperty>> sourceProperty,
        Expression<Func<T2, TProperty>> targetProperty
    )
    {
        FluentTypeInfo? sourceType = null;
        FluentTypeInfo? targetType = null;

        // Handle sourceProperty
        var sourceExpression =
            sourceProperty.Body as MemberExpression
            ?? (sourceProperty.Body as UnaryExpression)?.Operand as MemberExpression;

        if (sourceExpression != null)
        {
            var propertyName = sourceExpression.Member.Name;
            if (sourceOriginalFields.TryGetValue(propertyName, out FluentTypeInfo? value))
                sourceType = value;
        }

        // Handle targetProperty
        var targetExpression =
            targetProperty.Body as MemberExpression
            ?? (targetProperty.Body as UnaryExpression)?.Operand as MemberExpression;

        if (targetExpression != null)
        {
            var propertyName = targetExpression.Member.Name;
            if (targetOriginalFields.TryGetValue(propertyName, out FluentTypeInfo? value))
                targetType = value;
        }

        // Check if both types are resolved
        if (sourceType != null && targetType != null)
            Fields.Add(new(sourceType, targetType));
        else
            throw new Exception("Relation fields cannot be resolved");

        return this;
    }
}