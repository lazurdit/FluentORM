using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteConditionsManager<T> : IFluentConditionsManager<T> where T : IFluentModel, new()
    {
        public List<IFluentCondition> WhereConditions { get; } = new();

        public SQLiteConditionsManager()
        {
        }

        public virtual SQLiteConditionsManager<T> Clone(SQLiteConditionsManager<T> sourceConditionsManager)
        {
            WhereConditions.AddRange(new List<IFluentCondition>(sourceConditionsManager.WhereConditions));
            return this;
        }

        public virtual SQLiteConditionsManager<T> Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(SQLiteOp.Eq(property, value));
            return this;
        }

        public virtual SQLiteConditionsManager<T> Custom(IFluentSingleAttributeCondition condition)
        {
            WhereConditions.Add(condition);
            return this;
        }

        public virtual SQLiteConditionsManager<T> Or(Action<SQLiteConditionsManager<T>> orCallback)
        {
            FluentWhereConditionGroup conditionGroup = new()
            { CompareMethod = CompareMethods.Or };
            SQLiteConditionsManager<T> manager = new();
            orCallback(manager);
            conditionGroup.Conditions.AddRange(manager.WhereConditions);
            WhereConditions.Add(conditionGroup);
            return this;
        }

        public virtual SQLiteConditionsManager<T> And(Action<SQLiteConditionsManager<T>> orCallback)
        {
            FluentWhereConditionGroup conditionGroup = new()
            { CompareMethod = CompareMethods.And };
            SQLiteConditionsManager<T> manager = new();
            orCallback(manager);
            conditionGroup.Conditions.AddRange(manager.WhereConditions);
            WhereConditions.Add(conditionGroup);
            return this;
        }

        public virtual SQLiteConditionsManager<T> Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition)
        {
            WhereConditions.Add(SQLiteOp.Raw(property, whereCondition));
            return this;
        }

        public virtual SQLiteConditionsManager<T> Raw(string whereCondition)
        {
            WhereConditions.Add(new SQLiteRawCondition() { RawCondition = whereCondition });
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

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2) => Between(property, value1, value2);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Clone(IFluentConditionsManager<T> sourceIConditionsManager) => Clone((SQLiteConditionsManager<T>)sourceIConditionsManager);

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