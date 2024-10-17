using LazurdIT.FluentOrm.Common;
using System;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Oracle
{
    public class OracleFluentAggregateTypeInfoOp
    {
        private readonly FluentAggregateTypeInfo typeInfo;

        public OracleFluentAggregateTypeInfoOp(FluentAggregateTypeInfo typeInfo)
        {
            this.typeInfo = typeInfo;
        }

        public OracleIsNullCondition IsNull<T, TProperty>(Expression<Func<T, TProperty>> _) where T : IFluentModel
        {
            return new OracleIsNullCondition() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = true };
        }

        public OracleIsEqualCondition<T, TProperty> Eq<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new OracleIsEqualCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public OracleIsNullCondition IsNotNull<T, TProperty>(Expression<Func<T, TProperty>> _) where T : IFluentModel
        {
            return new OracleIsNullCondition() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = false };
        }

        public OracleRawCondition<T> Raw<T, TProperty>(Expression<Func<T, TProperty>> _, string value) where T : IFluentModel
        {
            return new OracleRawCondition<T>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public OracleIsBetweenCondition<T, TProperty> Between<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty values1, TProperty values2) where T : IFluentModel
        {
            return new OracleIsBetweenCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = values1, Value2 = values2, IsNotBetween = false };
        }

        public OracleIsBetweenCondition<T, TProperty> NotBetween<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty values1, TProperty values2) where T : IFluentModel
        {
            return new OracleIsBetweenCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = values1, Value2 = values2, IsNotBetween = true };
        }

        public OracleValueRangeCondition<T, TProperty> In<T, TProperty>(Expression<Func<T, TProperty>> _, params TProperty[] values) where T : IFluentModel
        {
            return new OracleValueRangeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Values = values };
        }

        public OracleValueRangeCondition<T, TProperty> NotIn<T, TProperty>(Expression<Func<T, TProperty>> _, params TProperty[] values) where T : IFluentModel
        {
            return new OracleValueRangeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Values = values, IsNotInRange = true };
        }

        public OracleIsLikeCondition<T, TProperty> Like<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new OracleIsLikeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public OracleIsLikeCondition<T, TProperty> NotLike<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new OracleIsLikeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, IsNotLike = true };
        }

        public OracleCutomOperatorCondition<T, TProperty> Gt<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new OracleCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = ">" };
        }

        public OracleCutomOperatorCondition<T, TProperty> Gte<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new OracleCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = ">=" };
        }

        public OracleCutomOperatorCondition<T, TProperty> Lt<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new OracleCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = "<" };
        }

        public OracleCutomOperatorCondition<T, TProperty> Lte<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new OracleCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = "<=" };
        }
    }
}