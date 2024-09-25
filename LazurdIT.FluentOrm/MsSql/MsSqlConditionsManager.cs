using LazurdIT.FluentOrm.Common;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.MsSql;

public class MsSqlConditionsManager<T> : IConditionsManager<T> where T : IFluentModel, new()
{
    public List<ICondition> WhereConditions { get; } =new();

    public MsSqlConditionsManager()
    {
    }

    public virtual MsSqlConditionsManager<T> Clone(MsSqlConditionsManager<T> sourceConditionsManager)
    {
        WhereConditions.AddRange(new List<ICondition>(sourceConditionsManager.WhereConditions));
        return this;
    }

    public virtual MsSqlConditionsManager<T> Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(MsSqlOp.Eq(property, value));
        return this;
    }

    public virtual MsSqlConditionsManager<T> Custom(ISingleAttributeCondition condition)
    {
        WhereConditions.Add(condition);
        return this;
    }

    public virtual MsSqlConditionsManager<T> Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition)
    {
        WhereConditions.Add(MsSqlOp.Raw(property, whereCondition));
        return this;
    }

    public virtual MsSqlConditionsManager<T> Raw(string whereCondition)
    {
        WhereConditions.Add(new MsSqlRawCondition() { Value = whereCondition });
        return this;
    }

    public MsSqlConditionsManager<T> Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2)
    {
        WhereConditions.Add(MsSqlOp.Between(property, value1, value2));
        return this;
    }

    public MsSqlConditionsManager<T> Gt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(MsSqlOp.Gt(property, value));
        return this;
    }

    public MsSqlConditionsManager<T> Gte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(MsSqlOp.Gte(property, value));
        return this;
    }

    public MsSqlConditionsManager<T> In<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values)
    {
        WhereConditions.Add(MsSqlOp.In(property, values));
        return this;
    }

    public MsSqlConditionsManager<T> IsNotNull<TProperty>(Expression<Func<T, TProperty>> property)
    {
        WhereConditions.Add(MsSqlOp.IsNotNull(property));
        return this;
    }

    public MsSqlConditionsManager<T> IsNull<TProperty>(Expression<Func<T, TProperty>> property)
    {
        WhereConditions.Add(MsSqlOp.IsNull(property));
        return this;
    }

    public MsSqlConditionsManager<T> Like<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(MsSqlOp.Like(property, value));
        return this;
    }

    public MsSqlConditionsManager<T> Lt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(MsSqlOp.Lt(property, value));
        return this;
    }

    public MsSqlConditionsManager<T> Lte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(MsSqlOp.Lte(property, value));
        return this;
    }

    public MsSqlConditionsManager<T> NotBetween<TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2)
    {
        WhereConditions.Add(MsSqlOp.NotBetween(property, values1, values2));
        return this;
    }

    public MsSqlConditionsManager<T> NotIn<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values)
    {
        WhereConditions.Add(MsSqlOp.NotIn(property, values));
        return this;
    }

    public MsSqlConditionsManager<T> NotLike<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(MsSqlOp.NotLike(property, value));
        return this;
    }

    IConditionsManager<T> IConditionsManager<T>.Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2) => Between(property, value1, value2);

    IConditionsManager<T> IConditionsManager<T>.Clone(IConditionsManager<T> sourceIConditionsManager) => Clone((MsSqlConditionsManager<T>)sourceIConditionsManager);

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