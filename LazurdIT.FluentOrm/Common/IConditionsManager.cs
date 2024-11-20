using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Common
{
    public interface IFluentConditionsManager<T> where T : IFluentModel, new()
    {
        List<IFluentCondition> WhereConditions { get; }

        IFluentConditionsManager<T> Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2);

        IFluentConditionsManager<T> Clone(IFluentConditionsManager<T> sourceIFluentConditionsManager);

        IFluentConditionsManager<T> Custom(IFluentSingleAttributeCondition condition);

        IFluentConditionsManager<T> Or(Action<IFluentConditionsManager<T>> condition);

        IFluentConditionsManager<T> And(Action<IFluentConditionsManager<T>> condition);

        IFluentConditionsManager<T> Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

        IFluentConditionsManager<T> Gt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

        IFluentConditionsManager<T> Gte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

        IFluentConditionsManager<T> In<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values);

        IFluentConditionsManager<T> IsNotNull<TProperty>(Expression<Func<T, TProperty>> property);

        IFluentConditionsManager<T> IsNull<TProperty>(Expression<Func<T, TProperty>> property);

        IFluentConditionsManager<T> Like<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

        IFluentConditionsManager<T> Lt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

        IFluentConditionsManager<T> Lte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

        IFluentConditionsManager<T> NotBetween<TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2);

        IFluentConditionsManager<T> NotIn<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values);

        IFluentConditionsManager<T> NotLike<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

        IFluentConditionsManager<T> Raw(string whereCondition);

        IFluentConditionsManager<T> Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition);
    }
}