using System;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Common
{
    public static class Ag
    {
        public static FluentAggregateTypeInfo ForField<T, TProperty>(Expression<Func<T, TProperty>> property, string? alias = null, AggregationMethods aggregationMethod = AggregationMethods.Count, string? customAggregationMethod = null) where T : IFluentModel
        {
            var originalFields = TypeCache.GetTypeCache<T>();
            if (property.Body is MemberExpression memberExpression)
            {
                var propertyName = memberExpression.Member.Name;
                if (originalFields.TryGetValue(propertyName, out FluentTypeInfo? value))
                    return new(value.Property, value.Attribute, aggregationMethod, alias, customAggregationMethod);
            }

            throw new ArgumentException("Property not found in the original fields.");
        }

        public static FluentAggregateTypeInfo SumForField<T, TProperty>(Expression<Func<T, TProperty>> property, string? alias = null) where T : IFluentModel
        {
            var originalFields = TypeCache.GetTypeCache<T>();
            if (property.Body is MemberExpression memberExpression)
            {
                var propertyName = memberExpression.Member.Name;
                if (originalFields.TryGetValue(propertyName, out FluentTypeInfo? value))
                    return new(value.Property, value.Attribute, AggregationMethods.Sum, alias);
            }

            throw new ArgumentException("Property not found in the original fields.");
        }

        public static FluentAggregateTypeInfo CountForField<T, TProperty>(Expression<Func<T, TProperty>> property, string? alias = null) where T : IFluentModel
        {
            var originalFields = TypeCache.GetTypeCache<T>();
            if (property.Body is MemberExpression memberExpression)
            {
                var propertyName = memberExpression.Member.Name;
                if (originalFields.TryGetValue(propertyName, out FluentTypeInfo? value))
                    return new(value.Property, value.Attribute, AggregationMethods.Count, alias);
            }

            throw new ArgumentException("Property not found in the original fields.");
        }

        public static FluentAggregateTypeInfo AvgForField<T, TProperty>(Expression<Func<T, TProperty>> property, string? alias = null) where T : IFluentModel
        {
            var originalFields = TypeCache.GetTypeCache<T>();
            if (property.Body is MemberExpression memberExpression)
            {
                var propertyName = memberExpression.Member.Name;
                if (originalFields.TryGetValue(propertyName, out FluentTypeInfo? value))
                    return new(value.Property, value.Attribute, AggregationMethods.Avg, alias);
            }

            throw new ArgumentException("Property not found in the original fields.");
        }

        public static FluentAggregateTypeInfo MinForField<T, TProperty>(Expression<Func<T, TProperty>> property, string? alias = null) where T : IFluentModel
        {
            var originalFields = TypeCache.GetTypeCache<T>();
            if (property.Body is MemberExpression memberExpression)
            {
                var propertyName = memberExpression.Member.Name;
                if (originalFields.TryGetValue(propertyName, out FluentTypeInfo? value))
                    return new(value.Property, value.Attribute, AggregationMethods.Min, alias);
            }

            throw new ArgumentException("Property not found in the original fields.");
        }

        public static FluentAggregateTypeInfo MaxForField<T, TProperty>(Expression<Func<T, TProperty>> property, string? alias = null) where T : IFluentModel
        {
            var originalFields = TypeCache.GetTypeCache<T>();
            if (property.Body is MemberExpression memberExpression)
            {
                var propertyName = memberExpression.Member.Name;
                if (originalFields.TryGetValue(propertyName, out FluentTypeInfo? value))
                    return new(value.Property, value.Attribute, AggregationMethods.Max, alias);
            }

            throw new ArgumentException("Property not found in the original fields.");
        }
    }
}