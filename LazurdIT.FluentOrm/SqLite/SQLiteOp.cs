using LazurdIT.FluentOrm.Common;
using System;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.SQLite
{
    public static class SQLiteOp
    {
        public static SQLiteIsNullCondition IsNull<T, TProperty>(Expression<Func<T, TProperty>> property) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteIsNullCondition() { AttributeName = propertyOrField, Value = true };
        }

        public static SQLiteIsEqualCondition<T, TProperty> Eq<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteIsEqualCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value };
        }

        public static SQLiteIsNullCondition IsNotNull<T, TProperty>(Expression<Func<T, TProperty>> property) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteIsNullCondition() { AttributeName = propertyOrField, Value = false };
        }

        public static SQLiteRawCondition<T> Raw<T, TProperty>(Expression<Func<T, TProperty>> property, string value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteRawCondition<T>() { AttributeName = propertyOrField, Value = value };
        }

        public static SQLiteIsBetweenCondition<T, TProperty> Between<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteIsBetweenCondition<T, TProperty>() { AttributeName = propertyOrField, Value = values1, Value2 = values2, IsNotBetween = false };
        }

        public static SQLiteIsBetweenCondition<T, TProperty> NotBetween<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteIsBetweenCondition<T, TProperty>() { AttributeName = propertyOrField, Value = values1, Value2 = values2, IsNotBetween = true };
        }

        public static SQLiteValueRangeCondition<T, TProperty> In<T, TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteValueRangeCondition<T, TProperty>() { AttributeName = propertyOrField, Values = values };
        }

        public static SQLiteValueRangeCondition<T, TProperty> NotIn<T, TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteValueRangeCondition<T, TProperty>() { AttributeName = propertyOrField, Values = values, IsNotInRange = true };
        }

        public static SQLiteIsLikeCondition<T, TProperty> Like<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteIsLikeCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value };
        }

        public static SQLiteIsLikeCondition<T, TProperty> NotLike<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteIsLikeCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, IsNotLike = true };
        }

        public static SQLiteCutomOperatorCondition<T, TProperty> Gt<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = ">" };
        }

        public static SQLiteCutomOperatorCondition<T, TProperty> Gte<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = ">=" };
        }

        public static SQLiteCutomOperatorCondition<T, TProperty> Lt<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = "<" };
        }

        public static SQLiteCutomOperatorCondition<T, TProperty> Lte<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new SQLiteCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = "<=" };
        }
    }
}