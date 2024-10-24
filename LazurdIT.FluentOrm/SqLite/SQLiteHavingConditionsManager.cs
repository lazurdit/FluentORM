using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteHavingConditionsManager<T> : IHavingConditionsManager<T> where T : IFluentModel, new()
    {
        public List<ICondition> HavingConditions { get; } = new();

        public SQLiteHavingConditionsManager()
        {
        }

        public SQLiteHavingConditionsManager<T> HavingAggregate<TProperty>(Expression<Func<T, TProperty>> property, Func<Expression<Func<T, TProperty>>, FluentAggregateTypeInfo> typeInfoFunc, Func<Expression<Func<T, TProperty>>, SQLiteFluentAggregateTypeInfoOp, ICondition> opFunc)
        {
            var typeInfo = typeInfoFunc(property);

            HavingConditions.Add(opFunc(property, new SQLiteFluentAggregateTypeInfoOp(typeInfo)));
            return this;
        }
    }
}