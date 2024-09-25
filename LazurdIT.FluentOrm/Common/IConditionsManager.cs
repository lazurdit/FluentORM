using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Common;

public interface IConditionsManager<T> where T : IFluentModel, new()
{
    List<ICondition> WhereConditions { get; }

    IConditionsManager<T> Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2);

    IConditionsManager<T> Clone(IConditionsManager<T> sourceIConditionsManager);

    IConditionsManager<T> Custom(ISingleAttributeCondition condition);

    IConditionsManager<T> Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

    IConditionsManager<T> Gt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

    IConditionsManager<T> Gte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

    IConditionsManager<T> In<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values);

    IConditionsManager<T> IsNotNull<TProperty>(Expression<Func<T, TProperty>> property);

    IConditionsManager<T> IsNull<TProperty>(Expression<Func<T, TProperty>> property);

    IConditionsManager<T> Like<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

    IConditionsManager<T> Lt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

    IConditionsManager<T> Lte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

    IConditionsManager<T> NotBetween<TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2);

    IConditionsManager<T> NotIn<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values);

    IConditionsManager<T> NotLike<TProperty>(Expression<Func<T, TProperty>> property, TProperty value);

    IConditionsManager<T> Raw(string whereCondition);

    IConditionsManager<T> Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition);
}
