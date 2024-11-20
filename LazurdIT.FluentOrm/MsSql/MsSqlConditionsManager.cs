using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlConditionsManager<T> : IFluentConditionsManager<T> where T : IFluentModel, new()
    {
        public List<IFluentCondition> WhereConditions { get; } = new();

        public MsSqlConditionsManager()
        {
        }

        public virtual MsSqlConditionsManager<T> Clone(MsSqlConditionsManager<T> sourceConditionsManager)
        {
            WhereConditions.AddRange(new List<IFluentCondition>(sourceConditionsManager.WhereConditions));
            return this;
        }

        public virtual MsSqlConditionsManager<T> Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(MsSqlOp.Eq(property, value));
            return this;
        }

        public virtual MsSqlConditionsManager<T> Custom(IFluentSingleAttributeCondition condition)
        {
            WhereConditions.Add(condition);
            return this;
        }

        public virtual MsSqlConditionsManager<T> Or(Action<MsSqlConditionsManager<T>> orCallback)
        {
            FluentWhereConditionGroup conditionGroup = new()
            { CompareMethod = CompareMethods.Or };
            MsSqlConditionsManager<T> manager = new();
            orCallback(manager);
            conditionGroup.Conditions.AddRange(manager.WhereConditions);
            WhereConditions.Add(conditionGroup);
            return this;
        }

        public virtual MsSqlConditionsManager<T> And(Action<MsSqlConditionsManager<T>> orCallback)
        {
            FluentWhereConditionGroup conditionGroup = new()
            { CompareMethod = CompareMethods.And };
            MsSqlConditionsManager<T> manager = new();
            orCallback(manager);
            conditionGroup.Conditions.AddRange(manager.WhereConditions);
            WhereConditions.Add(conditionGroup);
            return this;
        }

        public virtual MsSqlConditionsManager<T> Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition)
        {
            WhereConditions.Add(MsSqlOp.Raw(property, whereCondition));
            return this;
        }

        public virtual MsSqlConditionsManager<T> Raw(string whereCondition)
        {
            WhereConditions.Add(new MsSqlRawCondition() { RawCondition = whereCondition });
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

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2) => Between(property, value1, value2);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Clone(IFluentConditionsManager<T> sourceIFluentConditionsManager) => Clone((MsSqlConditionsManager<T>)sourceIFluentConditionsManager);

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