using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Oracle
{
    public class OracleHavingConditionsManager<T> : IHavingConditionsManager<T> where T : IFluentModel, new()
    {
        public List<ICondition> HavingConditions { get; } = new();

        public OracleHavingConditionsManager()
        {
        }

        public OracleHavingConditionsManager<T> HavingAggregate<TProperty>(Expression<Func<T, TProperty>> property, Func<Expression<Func<T, TProperty>>, FluentAggregateTypeInfo> typeInfoFunc, Func<Expression<Func<T, TProperty>>, OracleFluentAggregateTypeInfoOp, ICondition> opFunc)
        {
            var typeInfo = typeInfoFunc(property);

            HavingConditions.Add(opFunc(property, new OracleFluentAggregateTypeInfoOp(typeInfo)));
            return this;
        }
    }
}