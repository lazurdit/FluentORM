using LazurdIT.FluentOrm.Common;
using System;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlFluentAggregateTypeInfoOp
    {
        private readonly FluentAggregateTypeInfo typeInfo;

        public PgsqlFluentAggregateTypeInfoOp(FluentAggregateTypeInfo typeInfo)
        {
            this.typeInfo = typeInfo;
        }

        public PgsqlIsNullCondition IsNull<T, TProperty>(Expression<Func<T, TProperty>> _) where T : IFluentModel
        {
            return new PgsqlIsNullCondition() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = true };
        }

        public PgsqlIsEqualCondition<T, TProperty> Eq<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new PgsqlIsEqualCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public PgsqlIsNullCondition IsNotNull<T, TProperty>(Expression<Func<T, TProperty>> _) where T : IFluentModel
        {
            return new PgsqlIsNullCondition() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = false };
        }

        public PgsqlRawCondition<T> Raw<T, TProperty>(Expression<Func<T, TProperty>> _, string value) where T : IFluentModel
        {
            return new PgsqlRawCondition<T>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public PgsqlIsBetweenCondition<T, TProperty> Between<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty values1, TProperty values2) where T : IFluentModel
        {
            return new PgsqlIsBetweenCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = values1, Value2 = values2, IsNotBetween = false };
        }

        public PgsqlIsBetweenCondition<T, TProperty> NotBetween<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty values1, TProperty values2) where T : IFluentModel
        {
            return new PgsqlIsBetweenCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = values1, Value2 = values2, IsNotBetween = true };
        }

        public PgsqlValueRangeCondition<T, TProperty> In<T, TProperty>(Expression<Func<T, TProperty>> _, params TProperty[] values) where T : IFluentModel
        {
            return new PgsqlValueRangeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Values = values };
        }

        public PgsqlValueRangeCondition<T, TProperty> NotIn<T, TProperty>(Expression<Func<T, TProperty>> _, params TProperty[] values) where T : IFluentModel
        {
            return new PgsqlValueRangeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Values = values, IsNotInRange = true };
        }

        public PgsqlIsLikeCondition<T, TProperty> Like<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new PgsqlIsLikeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public PgsqlIsLikeCondition<T, TProperty> NotLike<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new PgsqlIsLikeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, IsNotLike = true };
        }

        public PgsqlCutomOperatorCondition<T, TProperty> Gt<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new PgsqlCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = ">" };
        }

        public PgsqlCutomOperatorCondition<T, TProperty> Gte<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new PgsqlCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = ">=" };
        }

        public PgsqlCutomOperatorCondition<T, TProperty> Lt<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new PgsqlCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = "<" };
        }

        public PgsqlCutomOperatorCondition<T, TProperty> Lte<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new PgsqlCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = "<=" };
        }
    }
}