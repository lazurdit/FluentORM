using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlHavingConditionsManager<T> : IHavingConditionsManager<T> where T : IFluentModel, new()
    {
        public List<ICondition> HavingConditions { get; } = new();

        public MsSqlHavingConditionsManager()
        {
        }

        public MsSqlHavingConditionsManager<T> HavingAggregate<TProperty>(Expression<Func<T, TProperty>> property, Func<Expression<Func<T, TProperty>>, FluentAggregateTypeInfo> typeInfoFunc, Func<Expression<Func<T, TProperty>>, MsSqlFluentAggregateTypeInfoOp, ICondition> opFunc)
        {
            var typeInfo = typeInfoFunc(property);

            HavingConditions.Add(opFunc(property, new MsSqlFluentAggregateTypeInfoOp(typeInfo)));
            return this;
        }
    }
}