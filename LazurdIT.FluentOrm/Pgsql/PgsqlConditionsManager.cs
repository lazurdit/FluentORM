using LazurdIT.FluentOrm.Common;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Pgsql;

public class PgsqlConditionsManager<T> : IConditionsManager<T> where T : IFluentModel, new()
{
    public List<ICondition> WhereConditions { get; } =new();

    public PgsqlConditionsManager()
    {
    }

    public virtual PgsqlConditionsManager<T> Clone(PgsqlConditionsManager<T> sourceConditionsManager)
    {
        WhereConditions.AddRange(new List<ICondition>(sourceConditionsManager.WhereConditions));
        return this;
    }

    public virtual PgsqlConditionsManager<T> Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(PgsqlOp.Eq(property, value));
        return this;
    }

    public virtual PgsqlConditionsManager<T> Custom(ISingleAttributeCondition condition)
    {
        WhereConditions.Add(condition);
        return this;
    }

    public virtual PgsqlConditionsManager<T> Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition)
    {
        WhereConditions.Add(PgsqlOp.Raw(property, whereCondition));
        return this;
    }

    public virtual PgsqlConditionsManager<T> Raw(string whereCondition)
    {
        WhereConditions.Add(new PgsqlRawCondition() { Value = whereCondition });
        return this;
    }

    public PgsqlConditionsManager<T> Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2)
    {
        WhereConditions.Add(PgsqlOp.Between(property, value1, value2));
        return this;
    }

    public PgsqlConditionsManager<T> Gt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(PgsqlOp.Gt(property, value));
        return this;
    }

    public PgsqlConditionsManager<T> Gte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(PgsqlOp.Gte(property, value));
        return this;
    }

    public PgsqlConditionsManager<T> In<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values)
    {
        WhereConditions.Add(PgsqlOp.In(property, values));
        return this;
    }

    public PgsqlConditionsManager<T> IsNotNull<TProperty>(Expression<Func<T, TProperty>> property)
    {
        WhereConditions.Add(PgsqlOp.IsNotNull(property));
        return this;
    }

    public PgsqlConditionsManager<T> IsNull<TProperty>(Expression<Func<T, TProperty>> property)
    {
        WhereConditions.Add(PgsqlOp.IsNull(property));
        return this;
    }

    public PgsqlConditionsManager<T> Like<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(PgsqlOp.Like(property, value));
        return this;
    }

    public PgsqlConditionsManager<T> Lt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(PgsqlOp.Lt(property, value));
        return this;
    }

    public PgsqlConditionsManager<T> Lte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(PgsqlOp.Lte(property, value));
        return this;
    }

    public PgsqlConditionsManager<T> NotBetween<TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2)
    {
        WhereConditions.Add(PgsqlOp.NotBetween(property, values1, values2));
        return this;
    }

    public PgsqlConditionsManager<T> NotIn<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values)
    {
        WhereConditions.Add(PgsqlOp.NotIn(property, values));
        return this;
    }

    public PgsqlConditionsManager<T> NotLike<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(PgsqlOp.NotLike(property, value));
        return this;
    }

    IConditionsManager<T> IConditionsManager<T>.Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2) => Between(property, value1, value2);

    IConditionsManager<T> IConditionsManager<T>.Clone(IConditionsManager<T> sourceIConditionsManager) => Clone((PgsqlConditionsManager<T>)sourceIConditionsManager);

    IConditionsManager<T> IConditionsManager<T>.Custom(ISingleAttributeCondition condition) => Custom(condition);

    IConditionsManager<T> IConditionsManager<T>.Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Eq(property, value);

    IConditionsManager<T> IConditionsManager<T>.Gt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Gt(property, value);

    IConditionsManager<T> IConditionsManager<T>.Gte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Gte(property, value);

    IConditionsManager<T> IConditionsManager<T>.In<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) => In(property, values);

    IConditionsManager<T> IConditionsManager<T>.IsNotNull<TProperty>(Expression<Func<T, TProperty>> property) => IsNotNull(property);

    IConditionsManager<T> IConditionsManager<T>.IsNull<TProperty>(Expression<Func<T, TProperty>> property) => IsNull(property);

    IConditionsManager<T> IConditionsManager<T>.Like<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Like(property, value);

    IConditionsManager<T> IConditionsManager<T>.Lt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Lt(property, value);

    IConditionsManager<T> IConditionsManager<T>.Lte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Lte(property, value);

    IConditionsManager<T> IConditionsManager<T>.NotBetween<TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2) => NotBetween(property, values1, values2);

    IConditionsManager<T> IConditionsManager<T>.NotIn<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) => NotIn(property, values);

    IConditionsManager<T> IConditionsManager<T>.NotLike<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => NotLike(property, value);

    IConditionsManager<T> IConditionsManager<T>.Raw(string whereCondition) => Raw(whereCondition);

    IConditionsManager<T> IConditionsManager<T>.Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition) => Raw(property, whereCondition);
}