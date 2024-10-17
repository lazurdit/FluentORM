using LazurdIT.FluentOrm.Common;
using System;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.MySql
{
    public static class MySqlOp
    {
        public static MySqlIsNullCondition IsNull<T, TProperty>(Expression<Func<T, TProperty>> property) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlIsNullCondition() { AttributeName = propertyOrField, Value = true };
        }

        public static MySqlIsEqualCondition<T, TProperty> Eq<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlIsEqualCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value };
        }

        public static MySqlIsNullCondition IsNotNull<T, TProperty>(Expression<Func<T, TProperty>> property) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlIsNullCondition() { AttributeName = propertyOrField, Value = false };
        }

        public static MySqlRawCondition<T> Raw<T, TProperty>(Expression<Func<T, TProperty>> property, string value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlRawCondition<T>() { AttributeName = propertyOrField, Value = value };
        }

        public static MySqlIsBetweenCondition<T, TProperty> Between<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlIsBetweenCondition<T, TProperty>() { AttributeName = propertyOrField, Value = values1, Value2 = values2, IsNotBetween = false };
        }

        public static MySqlIsBetweenCondition<T, TProperty> NotBetween<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlIsBetweenCondition<T, TProperty>() { AttributeName = propertyOrField, Value = values1, Value2 = values2, IsNotBetween = true };
        }

        public static MySqlValueRangeCondition<T, TProperty> In<T, TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlValueRangeCondition<T, TProperty>() { AttributeName = propertyOrField, Values = values };
        }

        public static MySqlValueRangeCondition<T, TProperty> NotIn<T, TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlValueRangeCondition<T, TProperty>() { AttributeName = propertyOrField, Values = values, IsNotInRange = true };
        }

        public static MySqlIsLikeCondition<T, TProperty> Like<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlIsLikeCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value };
        }

        public static MySqlIsLikeCondition<T, TProperty> NotLike<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlIsLikeCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, IsNotLike = true };
        }

        public static MySqlCutomOperatorCondition<T, TProperty> Gt<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = ">" };
        }

        public static MySqlCutomOperatorCondition<T, TProperty> Gte<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = ">=" };
        }

        public static MySqlCutomOperatorCondition<T, TProperty> Lt<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = "<" };
        }

        public static MySqlCutomOperatorCondition<T, TProperty> Lte<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new MySqlCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = "<=" };
        }
    }
}