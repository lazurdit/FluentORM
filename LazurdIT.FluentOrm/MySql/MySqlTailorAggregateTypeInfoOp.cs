using LazurdIT.FluentOrm.Common;
using System;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.MySql
{
    public class MySqlFluentAggregateTypeInfoOp
    {
        private readonly FluentAggregateTypeInfo typeInfo;

        public MySqlFluentAggregateTypeInfoOp(FluentAggregateTypeInfo typeInfo)
        {
            this.typeInfo = typeInfo;
        }

        public MySqlIsNullCondition IsNull<T, TProperty>(Expression<Func<T, TProperty>> _) where T : IFluentModel
        {
            return new MySqlIsNullCondition() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = true };
        }

        public MySqlIsEqualCondition<T, TProperty> Eq<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MySqlIsEqualCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public MySqlIsNullCondition IsNotNull<T, TProperty>(Expression<Func<T, TProperty>> _) where T : IFluentModel
        {
            return new MySqlIsNullCondition() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = false };
        }

        public MySqlRawCondition<T> Raw<T, TProperty>(Expression<Func<T, TProperty>> _, string value) where T : IFluentModel
        {
            return new MySqlRawCondition<T>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public MySqlIsBetweenCondition<T, TProperty> Between<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty values1, TProperty values2) where T : IFluentModel
        {
            return new MySqlIsBetweenCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = values1, Value2 = values2, IsNotBetween = false };
        }

        public MySqlIsBetweenCondition<T, TProperty> NotBetween<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty values1, TProperty values2) where T : IFluentModel
        {
            return new MySqlIsBetweenCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = values1, Value2 = values2, IsNotBetween = true };
        }

        public MySqlValueRangeCondition<T, TProperty> In<T, TProperty>(Expression<Func<T, TProperty>> _, params TProperty[] values) where T : IFluentModel
        {
            return new MySqlValueRangeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Values = values };
        }

        public MySqlValueRangeCondition<T, TProperty> NotIn<T, TProperty>(Expression<Func<T, TProperty>> _, params TProperty[] values) where T : IFluentModel
        {
            return new MySqlValueRangeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Values = values, IsNotInRange = true };
        }

        public MySqlIsLikeCondition<T, TProperty> Like<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MySqlIsLikeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public MySqlIsLikeCondition<T, TProperty> NotLike<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MySqlIsLikeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, IsNotLike = true };
        }

        public MySqlCutomOperatorCondition<T, TProperty> Gt<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MySqlCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = ">" };
        }

        public MySqlCutomOperatorCondition<T, TProperty> Gte<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MySqlCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = ">=" };
        }

        public MySqlCutomOperatorCondition<T, TProperty> Lt<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MySqlCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = "<" };
        }

        public MySqlCutomOperatorCondition<T, TProperty> Lte<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new MySqlCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = "<=" };
        }
    }
}