using System.Linq.Expressions;
using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.Pgsql;

public class PgsqlHavingConditionsManager<T> : IHavingConditionsManager<T> where T : IFluentModel, new()
{
    public List<ICondition> HavingConditions { get; } = new();

    public PgsqlHavingConditionsManager()
    {
    }

    public PgsqlHavingConditionsManager<T> HavingAggregate<TProperty>(Expression<Func<T, TProperty>> property, Func<Expression<Func<T, TProperty>>, FluentAggregateTypeInfo> typeInfoFunc, Func<Expression<Func<T, TProperty>>, PgsqlFluentAggregateTypeInfoOp, ICondition> opFunc)
    {
        var typeInfo = typeInfoFunc(property);
        HavingConditions.Add(opFunc(property, new PgsqlFluentAggregateTypeInfoOp(typeInfo)));
        return this;
    }
}