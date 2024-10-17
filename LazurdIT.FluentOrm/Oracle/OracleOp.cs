using LazurdIT.FluentOrm.Common;
using System;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Oracle
{
    public static class OracleOp
    {
        public static OracleIsNullCondition IsNull<T, TProperty>(Expression<Func<T, TProperty>> property) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleIsNullCondition() { AttributeName = propertyOrField, Value = true };
        }

        public static OracleIsEqualCondition<T, TProperty> Eq<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleIsEqualCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value };
        }

        public static OracleIsNullCondition IsNotNull<T, TProperty>(Expression<Func<T, TProperty>> property) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleIsNullCondition() { AttributeName = propertyOrField, Value = false };
        }

        public static OracleRawCondition<T> Raw<T, TProperty>(Expression<Func<T, TProperty>> property, string value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleRawCondition<T>() { AttributeName = propertyOrField, Value = value };
        }

        public static OracleIsBetweenCondition<T, TProperty> Between<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleIsBetweenCondition<T, TProperty>() { AttributeName = propertyOrField, Value = values1, Value2 = values2, IsNotBetween = false };
        }

        public static OracleIsBetweenCondition<T, TProperty> NotBetween<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleIsBetweenCondition<T, TProperty>() { AttributeName = propertyOrField, Value = values1, Value2 = values2, IsNotBetween = true };
        }

        public static OracleValueRangeCondition<T, TProperty> In<T, TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleValueRangeCondition<T, TProperty>() { AttributeName = propertyOrField, Values = values };
        }

        public static OracleValueRangeCondition<T, TProperty> NotIn<T, TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleValueRangeCondition<T, TProperty>() { AttributeName = propertyOrField, Values = values, IsNotInRange = true };
        }

        public static OracleIsLikeCondition<T, TProperty> Like<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleIsLikeCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value };
        }

        public static OracleIsLikeCondition<T, TProperty> NotLike<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleIsLikeCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, IsNotLike = true };
        }

        public static OracleCutomOperatorCondition<T, TProperty> Gt<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = ">" };
        }

        public static OracleCutomOperatorCondition<T, TProperty> Gte<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = ">=" };
        }

        public static OracleCutomOperatorCondition<T, TProperty> Lt<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = "<" };
        }

        public static OracleCutomOperatorCondition<T, TProperty> Lte<T, TProperty>(Expression<Func<T, TProperty>> property, TProperty value) where T : IFluentModel
        {
            string propertyOrField = AttributeResolver.ResolveFieldName(property);
            return new OracleCutomOperatorCondition<T, TProperty>() { AttributeName = propertyOrField, Value = value, Operator = "<=" };
        }
    }
}