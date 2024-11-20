using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.MySql
{
    public class MySqlConditionsManager<T> : IFluentConditionsManager<T> where T : IFluentModel, new()
    {
        public List<IFluentCondition> WhereConditions { get; } = new();

        public MySqlConditionsManager()
        {
        }

        public virtual MySqlConditionsManager<T> Clone(MySqlConditionsManager<T> sourceConditionsManager)
        {
            WhereConditions.AddRange(new List<IFluentCondition>(sourceConditionsManager.WhereConditions));
            return this;
        }

        public virtual MySqlConditionsManager<T> Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(MySqlOp.Eq(property, value));
            return this;
        }

        public virtual MySqlConditionsManager<T> Custom(IFluentSingleAttributeCondition condition)
        {
            WhereConditions.Add(condition);
            return this;
        }

        public virtual MySqlConditionsManager<T> Or(Action<MySqlConditionsManager<T>> orCallback)
        {
            FluentWhereConditionGroup conditionGroup = new()
            { CompareMethod = CompareMethods.Or };
            MySqlConditionsManager<T> manager = new();
            orCallback(manager);
            conditionGroup.Conditions.AddRange(manager.WhereConditions);
            WhereConditions.Add(conditionGroup);
            return this;
        }

        public virtual MySqlConditionsManager<T> And(Action<MySqlConditionsManager<T>> orCallback)
        {
            FluentWhereConditionGroup conditionGroup = new()
            { CompareMethod = CompareMethods.And };
            MySqlConditionsManager<T> manager = new();
            orCallback(manager);
            conditionGroup.Conditions.AddRange(manager.WhereConditions);
            WhereConditions.Add(conditionGroup);
            return this;
        }

        public virtual MySqlConditionsManager<T> Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition)
        {
            WhereConditions.Add(MySqlOp.Raw(property, whereCondition));
            return this;
        }

        public virtual MySqlConditionsManager<T> Raw(string whereCondition)
        {
            WhereConditions.Add(new MySqlRawCondition() { RawCondition = whereCondition });
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

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2) => Between(property, value1, value2);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Clone(IFluentConditionsManager<T> sourceIConditionsManager) => Clone((MySqlConditionsManager<T>)sourceIConditionsManager);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Custom(IFluentSingleAttributeCondition condition) => Custom(condition);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.And(Action<IFluentConditionsManager<T>> conditionGroup) => And(conditionGroup);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Or(Action<IFluentConditionsManager<T>> conditionGroup) => Or(conditionGroup);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Eq(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Gt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Gt(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Gte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Gte(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.In<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) => In(property, values);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.IsNotNull<TProperty>(Expression<Func<T, TProperty>> property) => IsNotNull(property);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.IsNull<TProperty>(Expression<Func<T, TProperty>> property) => IsNull(property);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Like<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Like(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Lt<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Lt(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Lte<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => Lte(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.NotBetween<TProperty>(Expression<Func<T, TProperty>> property, TProperty values1, TProperty values2) => NotBetween(property, values1, values2);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.NotIn<TProperty>(Expression<Func<T, TProperty>> property, params TProperty[] values) => NotIn(property, values);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.NotLike<TProperty>(Expression<Func<T, TProperty>> property, TProperty value) => NotLike(property, value);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Raw(string whereCondition) => Raw(whereCondition);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition) => Raw(property, whereCondition);
    }
}