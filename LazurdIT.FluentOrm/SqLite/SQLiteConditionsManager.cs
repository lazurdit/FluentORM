using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteConditionsManager<T> : IConditionsManager<T> where T : IFluentModel, new()
    {
        public List<ICondition> WhereConditions { get; } = new();

        public SQLiteConditionsManager()
        {
        }

        public virtual SQLiteConditionsManager<T> Clone(SQLiteConditionsManager<T> sourceConditionsManager)
        {
            WhereConditions.AddRange(new List<ICondition>(sourceConditionsManager.WhereConditions));
            return this;
        }

        public virtual SQLiteConditionsManager<T> Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(SQLiteOp.Eq(property, value));
            return this;
        }

        public virtual SQLiteConditionsManager<T> Custom(ISingleAttributeCondition condition)
        {
            WhereConditions.Add(condition);
            return this;
        }

        public virtual SQLiteConditionsManager<T> Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition)
        {
            WhereConditions.Add(SQLiteOp.Raw(property, whereCondition));
            return this;
        }

        public virtual SQLiteConditionsManager<T> Raw(string whereCondition)
        {
            WhereConditions.Add(new SQLiteRawCondition() { Value = whereCondition });
            return this;
        }

        public SQLiteConditionsManager<T> Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2)
        {
            WhereConditions.Add(SQLiteOp.Between(property, value1, value2));
            return this;
        }

        public SQLiteConditionsManager<T> Gt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(SQLiteOp.Gt(property, value));
            return this;
        }

        public SQLiteConditionsManager<T> Gte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(SQLiteOp.Gte(property, value));
            return this;
        }

        public SQLiteConditionsManager<T> In<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values)
        {
            WhereConditions.Add(SQLiteOp.In(property, values));
            return this;
        }

        public SQLiteConditionsManager<T> IsNotNull<TProperty>(Expression<Func<T, TProperty>> property)
        {
            WhereConditions.Add(SQLiteOp.IsNotNull(property));
            return this;
        }

        public SQLiteConditionsManager<T> IsNull<TProperty>(Expression<Func<T, TProperty>> property)
        {
            WhereConditions.Add(SQLiteOp.IsNull(property));
            return this;
        }

        public SQLiteConditionsManager<T> Like<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(SQLiteOp.Like(property, value));
            return this;
        }

        public SQLiteConditionsManager<T> Lt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(SQLiteOp.Lt(property, value));
            return this;
        }

        public SQLiteConditionsManager<T> Lte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(SQLiteOp.Lte(property, value));
            return this;
        }

        public SQLiteConditionsManager<T> NotBetween<TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2)
        {
            WhereConditions.Add(SQLiteOp.NotBetween(property, values1, values2));
            return this;
        }

        public SQLiteConditionsManager<T> NotIn<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values)
        {
            WhereConditions.Add(SQLiteOp.NotIn(property, values));
            return this;
        }

        public SQLiteConditionsManager<T> NotLike<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(SQLiteOp.NotLike(property, value));
            return this;
        }

        IConditionsManager<T> IConditionsManager<T>.Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2) => Between(property, value1, value2);

        IConditionsManager<T> IConditionsManager<T>.Clone(IConditionsManager<T> sourceIConditionsManager) => Clone((SQLiteConditionsManager<T>)sourceIConditionsManager);

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