using LazurdIT.FluentOrm.Common;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.MsSql;

public static class MsSqlOp
{
    public static MsSqlIsNullCondition IsNull<T, TProperty>(Expression<Func<T, TProperty>> property) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlIsNullCondition() { AttributeName = propertyOrField, Value = true };
    }

    public static MsSqlIsEqualCondition<T, TProperty> Eq<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlIsEqualCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value };
    }

    public static MsSqlIsNullCondition IsNotNull<T, TProperty>(Expression<Func<T, TProperty>> property) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlIsNullCondition() { AttributeName = propertyOrField, Value = false };
    }

    public static MsSqlRawCondition<T> Raw<T, TProperty>(Expression<Func<T, TProperty>> property, string value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlRawCondition<T>() { AttributeName = propertyOrField, Value = value };
    }

    public static MsSqlIsBetweenCondition<T, TProperty> Between<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlIsBetweenCondition<T, TProperty>() { AttributeName = propertyOrField, Value = values1, Value2 = values2, IsNotBetween = false };
    }

    public static MsSqlIsBetweenCondition<T, TProperty> NotBetween<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlIsBetweenCondition<T, TProperty>() { AttributeName = propertyOrField, Value = values1, Value2 = values2, IsNotBetween = true };
    }

    public static MsSqlValueRangeCondition<T, TProperty> In<T, TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlValueRangeCondition<T, TProperty>() { AttributeName = propertyOrField, Values = values };
    }

    public static MsSqlValueRangeCondition<T, TProperty> NotIn<T, TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlValueRangeCondition<T, TProperty>() { AttributeName = propertyOrField, Values = values, IsNotInRange = true };
    }

    public static MsSqlIsLikeCondition<T, TProperty> Like<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlIsLikeCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value };
    }

    public static MsSqlIsLikeCondition<T, TProperty> NotLike<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlIsLikeCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, IsNotLike = true };
    }

    public static MsSqlCutomOperatorCondition<T, TProperty> Gt<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = ">" };
    }

    public static MsSqlCutomOperatorCondition<T, TProperty> Gte<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = ">=" };
    }

    public static MsSqlCutomOperatorCondition<T, TProperty> Lt<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = "<" };
    }

    public static MsSqlCutomOperatorCondition<T, TProperty> Lte<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
    {
        string propertyOrField = AttributeResolver.ResolveFieldName(property);
        return new MsSqlCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = "<=" };
    }
}