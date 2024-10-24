using LazurdIT.FluentOrm.Common;
using System;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteFluentAggregateTypeInfoOp
    {
        private readonly FluentAggregateTypeInfo typeInfo;

        public SQLiteFluentAggregateTypeInfoOp(FluentAggregateTypeInfo typeInfo)
        {
            this.typeInfo = typeInfo;
        }

        public SQLiteIsNullCondition IsNull<T, TProperty>(Expression<Func<T, TProperty>> _) where T : IFluentModel
        {
            return new SQLiteIsNullCondition() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = true };
        }

        public SQLiteIsEqualCondition<T, TProperty> Eq<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new SQLiteIsEqualCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public SQLiteIsNullCondition IsNotNull<T, TProperty>(Expression<Func<T, TProperty>> _) where T : IFluentModel
        {
            return new SQLiteIsNullCondition() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = false };
        }

        public SQLiteRawCondition<T> Raw<T, TProperty>(Expression<Func<T, TProperty>> _, string value) where T : IFluentModel
        {
            return new SQLiteRawCondition<T>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public SQLiteIsBetweenCondition<T, TProperty> Between<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty values1, TProperty values2) where T : IFluentModel
        {
            return new SQLiteIsBetweenCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = values1, Value2 = values2, IsNotBetween = false };
        }

        public SQLiteIsBetweenCondition<T, TProperty> NotBetween<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty values1, TProperty values2) where T : IFluentModel
        {
            return new SQLiteIsBetweenCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = values1, Value2 = values2, IsNotBetween = true };
        }

        public SQLiteValueRangeCondition<T, TProperty> In<T, TProperty>(Expression<Func<T, TProperty>> _, params TProperty[] values) where T : IFluentModel
        {
            return new SQLiteValueRangeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Values = values };
        }

        public SQLiteValueRangeCondition<T, TProperty> NotIn<T, TProperty>(Expression<Func<T, TProperty>> _, params TProperty[] values) where T : IFluentModel
        {
            return new SQLiteValueRangeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Values = values, IsNotInRange = true };
        }

        public SQLiteIsLikeCondition<T, TProperty> Like<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new SQLiteIsLikeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value };
        }

        public SQLiteIsLikeCondition<T, TProperty> NotLike<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new SQLiteIsLikeCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, IsNotLike = true };
        }

        public SQLiteCutomOperatorCondition<T, TProperty> Gt<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new SQLiteCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = ">" };
        }

        public SQLiteCutomOperatorCondition<T, TProperty> Gte<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new SQLiteCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = ">=" };
        }

        public SQLiteCutomOperatorCondition<T, TProperty> Lt<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new SQLiteCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = "<" };
        }

        public SQLiteCutomOperatorCondition<T, TProperty> Lte<T, TProperty>(Expression<Func<T, TProperty>> _, TProperty value) where T : IFluentModel
        {
            return new SQLiteCutomOperatorCondition<T, TProperty>() { AttributeName = typeInfo.GetExpressionOnly(), ParameterName = $"agg_{typeInfo.FinalPropertyName}", Value = value, Operator = "<=" };
        }
    }
}