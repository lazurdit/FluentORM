using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.MySql
{
    public class MySqlHavingConditionsManager<T> : IHavingConditionsManager<T> where T : IFluentModel, new()
    {
        public List<IFluentCondition> HavingConditions { get; } = new();

        public MySqlHavingConditionsManager()
        {
        }

        public MySqlHavingConditionsManager<T> HavingAggregate<TProperty>(Expression<Func<T, TProperty>> property, Func<Expression<Func<T, TProperty>>, FluentAggregateTypeInfo> typeInfoFunc, Func<Expression<Func<T, TProperty>>, MySqlFluentAggregateTypeInfoOp, IFluentCondition> opFunc)
        {
            var typeInfo = typeInfoFunc(property);

            HavingConditions.Add(opFunc(property, new MySqlFluentAggregateTypeInfoOp(typeInfo)));
            return this;
        }
    }
}