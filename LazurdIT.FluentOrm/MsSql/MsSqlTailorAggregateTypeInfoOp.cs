using LazurdIT.FluentOrm.Common;
using System;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlFluentAggregateTypeInfoOp
    {
        private readonly FluentAggregateTypeInfo typeInfo;

        public MsSqlFluentAggregateTypeInfoOp(FluentAggregateTypeInfo typeInfo)
        {
            this.typeInfo = typeInfo;
        }

        public MsSqlIsNullCondition IsNull<T, TProperty>(Expression<Func<T, TProperty>> _) where T : IFluentModel
        {
            return new MsSqlIsNullCondition() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = true };
        }

        public MsSqlIsEqualCondition<T, TProperty> Eq<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MsSqlIsEqualCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public MsSqlIsNullCondition IsNotNull<T, TProperty>(Expression<Func<T, TProperty>> _) where T : IFluentModel
        {
            return new MsSqlIsNullCondition() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = false };
        }

        public MsSqlRawCondition<T> Raw<T, TProperty>(Expression<Func<T, TProperty>> _, string value) where T : IFluentModel
        {
            return new MsSqlRawCondition<T>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public MsSqlIsBetweenCondition<T, TProperty> Between<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty values1, TProperty values2) where T : IFluentModel
        {
            return new MsSqlIsBetweenCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = values1, Value2 = values2, IsNotBetween = false };
        }

        public MsSqlIsBetweenCondition<T, TProperty> NotBetween<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty values1, TProperty values2) where T : IFluentModel
        {
            return new MsSqlIsBetweenCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = values1, Value2 = values2, IsNotBetween = true };
        }

        public MsSqlValueRangeCondition<T, TProperty> In<T, TProperty>(Expression<Func<T, TProperty>> _, params TProperty[] values) where T : IFluentModel
        {
            return new MsSqlValueRangeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Values = values };
        }

        public MsSqlValueRangeCondition<T, TProperty> NotIn<T, TProperty>(Expression<Func<T, TProperty>> _, params TProperty[] values) where T : IFluentModel
        {
            return new MsSqlValueRangeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Values = values, IsNotInRange = true };
        }

        public MsSqlIsLikeCondition<T, TProperty> Like<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MsSqlIsLikeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public MsSqlIsLikeCondition<T, TProperty> NotLike<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MsSqlIsLikeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, IsNotLike = true };
        }

        public MsSqlCutomOperatorCondition<T, TProperty> Gt<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MsSqlCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = ">" };
        }

        public MsSqlCutomOperatorCondition<T, TProperty> Gte<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MsSqlCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = ">=" };
        }

        public MsSqlCutomOperatorCondition<T, TProperty> Lt<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MsSqlCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = "<" };
        }

        public MsSqlCutomOperatorCondition<T, TProperty> Lte<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MsSqlCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = "<=" };
        }
    }
}