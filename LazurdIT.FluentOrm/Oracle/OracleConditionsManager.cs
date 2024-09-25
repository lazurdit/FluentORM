using LazurdIT.FluentOrm.Common;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Oracle;

public class OracleConditionsManager<T> : IConditionsManager<T> where T : IFluentModel, new()
{
    public List<ICondition> WhereConditions { get; } = new();

    public OracleConditionsManager()
    {
    }

    public virtual OracleConditionsManager<T> Clone(OracleConditionsManager<T> sourceConditionsManager)
    {
        WhereConditions.AddRange(new List<ICondition>(sourceConditionsManager.WhereConditions));
        return this;
    }

    public virtual OracleConditionsManager<T> Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(OracleOp.Eq(property, value));
        return this;
    }

    public virtual OracleConditionsManager<T> Custom(ISingleAttributeCondition condition)
    {
        WhereConditions.Add(condition);
        return this;
    }

    public virtual OracleConditionsManager<T> Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition)
    {
        WhereConditions.Add(OracleOp.Raw(property, whereCondition));
        return this;
    }

    public virtual OracleConditionsManager<T> Raw(string whereCondition)
    {
        WhereConditions.Add(new OracleRawCondition() { Value = whereCondition });
        return this;
    }

    public OracleConditionsManager<T> Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2)
    {
        WhereConditions.Add(OracleOp.Between(property, value1, value2));
        return this;
    }

    public OracleConditionsManager<T> Gt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(OracleOp.Gt(property, value));
        return this;
    }

    public OracleConditionsManager<T> Gte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(OracleOp.Gte(property, value));
        return this;
    }

    public OracleConditionsManager<T> In<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values)
    {
        WhereConditions.Add(OracleOp.In(property, values));
        return this;
    }

    public OracleConditionsManager<T> IsNotNull<TProperty>(Expression<Func<T, TProperty>> property)
    {
        WhereConditions.Add(OracleOp.IsNotNull(property));
        return this;
    }

    public OracleConditionsManager<T> IsNull<TProperty>(Expression<Func<T, TProperty>> property)
    {
        WhereConditions.Add(OracleOp.IsNull(property));
        return this;
    }

    public OracleConditionsManager<T> Like<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(OracleOp.Like(property, value));
        return this;
    }

    public OracleConditionsManager<T> Lt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(OracleOp.Lt(property, value));
        return this;
    }

    public OracleConditionsManager<T> Lte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(OracleOp.Lte(property, value));
        return this;
    }

    public OracleConditionsManager<T> NotBetween<TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2)
    {
        WhereConditions.Add(OracleOp.NotBetween(property, values1, values2));
        return this;
    }

    public OracleConditionsManager<T> NotIn<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values)
    {
        WhereConditions.Add(OracleOp.NotIn(property, values));
        return this;
    }

    public OracleConditionsManager<T> NotLike<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
    {
        WhereConditions.Add(OracleOp.NotLike(property, value));
        return this;
    }

    IConditionsManager<T> IConditionsManager<T>.Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2) => Between(property, value1, value2);

    IConditionsManager<T> IConditionsManager<T>.Clone(IConditionsManager<T> sourceIConditionsManager) => Clone((OracleConditionsManager<T>)sourceIConditionsManager);

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