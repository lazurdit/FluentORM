using System.Linq.Expressions;
using System.Reflection;

namespace LazurdIT.FluentOrm.Common;

public static class AttributeResolver
{
    public static string ResolveTableName<T>() where T : IFluentModel, new()
    {
        var attribute = typeof(T).GetCustomAttribute<FluentTableAttribute>();
        string name = attribute?.Name ?? typeof(T).Name;
        return name;
    }

    public static string ResolveFieldName<T, TProperty>(Expression<Func<T, TProperty>> targetProperty) where T : IFluentModel
    {
        // Get the property name from the expression
        if (targetProperty.Body is MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;

            // Retrieve the type-specific cache
            var typeCache = TypeCache.GetTypeCache<T>();

            if (typeCache.TryGetValue(propertyName, out var propertyInfo))
                return propertyInfo.FinalPropertyName;
        }

        return string.Empty;
    }

    public static string ResolvePropertyName<T, TProperty>(Expression<Func<T, TProperty>> targetProperty) where T : IFluentModel
    {
        // Get the property name from the expression
        if (targetProperty.Body is MemberExpression memberExpression)
            return memberExpression.Member.Name;

        return targetProperty.Name ?? string.Empty;
    }
}