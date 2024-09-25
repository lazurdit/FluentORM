using LazurdIT.FluentOrm.Common;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Pgsql;

public static class PgsqlOp
{
    public static PgsqlIsNullCondition IsNull<T, TProperty>(Expression<Func<T, TProperty>> property) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlIsNullCondition() { AttributeName = propertyOrField, Value = true };
    }

    public static PgsqlIsEqualCondition<T, TProperty> Eq<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlIsEqualCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value };
    }

    public static PgsqlIsNullCondition IsNotNull<T, TProperty>(Expression<Func<T, TProperty>> property) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlIsNullCondition() { AttributeName = propertyOrField, Value = false };
    }

    public static PgsqlRawCondition<T> Raw<T, TProperty>(Expression<Func<T, TProperty>> property, string value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlRawCondition<T>() { AttributeName = propertyOrField, Value = value };
    }

    public static PgsqlIsBetweenCondition<T, TProperty> Between<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlIsBetweenCondition<T, TProperty>() { AttributeName = propertyOrField, Value = values1, Value2 = values2, IsNotBetween = false };
    }

    public static PgsqlIsBetweenCondition<T, TProperty> NotBetween<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlIsBetweenCondition<T, TProperty>() { AttributeName = propertyOrField, Value = values1, Value2 = values2, IsNotBetween = true };
    }

    public static PgsqlValueRangeCondition<T, TProperty> In<T, TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlValueRangeCondition<T, TProperty>() { AttributeName = propertyOrField, Values = values };
    }

    public static PgsqlValueRangeCondition<T, TProperty> NotIn<T, TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlValueRangeCondition<T, TProperty>() { AttributeName = propertyOrField, Values = values, IsNotInRange = true };
    }

    public static PgsqlIsLikeCondition<T, TProperty> Like<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlIsLikeCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value };
    }

    public static PgsqlIsLikeCondition<T, TProperty> NotLike<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlIsLikeCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, IsNotLike = true };
    }

    public static PgsqlCutomOperatorCondition<T, TProperty> Gt<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = ">" };
    }

    public static PgsqlCutomOperatorCondition<T, TProperty> Gte<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = ">=" };
    }

    public static PgsqlCutomOperatorCondition<T, TProperty> Lt<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = "<" };
    }

    public static PgsqlCutomOperatorCondition<T, TProperty> Lte<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new PgsqlCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = "<=" };
    }
}