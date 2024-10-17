using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.MySql
{
    public class MySqlConditionsManager<T> : IConditionsManager<T> where T : IFluentModel, new()
    {
        public List<ICondition> WhereConditions { get; } = new();

        public MySqlConditionsManager()
        {
        }

        public virtual MySqlConditionsManager<T> Clone(MySqlConditionsManager<T> sourceConditionsManager)
        {
            WhereConditions.AddRange(new List<ICondition>(sourceConditionsManager.WhereConditions));
            return this;
        }

        public virtual MySqlConditionsManager<T> Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(MySqlOp.Eq(property, value));
            return this;
        }

        public virtual MySqlConditionsManager<T> Custom(ISingleAttributeCondition condition)
        {
            WhereConditions.Add(condition);
            return this;
        }

        public virtual MySqlConditionsManager<T> Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition)
        {
            WhereConditions.Add(MySqlOp.Raw(property, whereCondition));
            return this;
        }

        public virtual MySqlConditionsManager<T> Raw(string whereCondition)
        {
            WhereConditions.Add(new MySqlRawCondition() { Value = whereCondition });
            return this;
        }

        public MySqlConditionsManager<T> Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2)
        {
            WhereConditions.Add(MySqlOp.Between(property, value1, value2));
            return this;
        }

        public MySqlConditionsManager<T> Gt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(MySqlOp.Gt(property, value));
            return this;
        }

        public MySqlConditionsManager<T> Gte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(MySqlOp.Gte(property, value));
            return this;
        }

        public MySqlConditionsManager<T> In<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values)
        {
            WhereConditions.Add(MySqlOp.In(property, values));
            return this;
        }

        public MySqlConditionsManager<T> IsNotNull<TProperty>(Expression<Func<T, TProperty>> property)
        {
            WhereConditions.Add(MySqlOp.IsNotNull(property));
            return this;
        }

        public MySqlConditionsManager<T> IsNull<TProperty>(Expression<Func<T, TProperty>> property)
        {
            WhereConditions.Add(MySqlOp.IsNull(property));
            return this;
        }

        public MySqlConditionsManager<T> Like<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(MySqlOp.Like(property, value));
            return this;
        }

        public MySqlConditionsManager<T> Lt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(MySqlOp.Lt(property, value));
            return this;
        }

        public MySqlConditionsManager<T> Lte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(MySqlOp.Lte(property, value));
            return this;
        }

        public MySqlConditionsManager<T> NotBetween<TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2)
        {
            WhereConditions.Add(MySqlOp.NotBetween(property, values1, values2));
            return this;
        }

        public MySqlConditionsManager<T> NotIn<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values)
        {
            WhereConditions.Add(MySqlOp.NotIn(property, values));
            return this;
        }

        public MySqlConditionsManager<T> NotLike<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(MySqlOp.NotLike(property, value));
            return this;
        }

        IConditionsManager<T> IConditionsManager<T>.Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2) => Between(property, value1, value2);

        IConditionsManager<T> IConditionsManager<T>.Clone(IConditionsManager<T> sourceIConditionsManager) => Clone((MySqlConditionsManager<T>)sourceIConditionsManager);

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
}