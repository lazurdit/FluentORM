using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Oracle
{
    public class OracleConditionsManager<T> : IFluentConditionsManager<T> where T : IFluentModel, new()
    {
        public List<IFluentCondition> WhereConditions { get; } = new();

        public OracleConditionsManager()
        {
        }

        public virtual OracleConditionsManager<T> Clone(OracleConditionsManager<T> sourceConditionsManager)
        {
            WhereConditions.AddRange(new List<IFluentCondition>(sourceConditionsManager.WhereConditions));
            return this;
        }

        public virtual OracleConditionsManager<T> Eq<TProperty>(Expression<Func<T, TProperty>> property, TProperty value)
        {
            WhereConditions.Add(OracleOp.Eq(property, value));
            return this;
        }

        public virtual OracleConditionsManager<T> Custom(IFluentSingleAttributeCondition condition)
        {
            WhereConditions.Add(condition);
            return this;
        }

        public virtual OracleConditionsManager<T> Or(Action<OracleConditionsManager<T>> orCallback)
        {
            FluentWhereConditionGroup conditionGroup = new()
            { CompareMethod = CompareMethods.Or };
            OracleConditionsManager<T> manager = new();
            orCallback(manager);
            conditionGroup.Conditions.AddRange(manager.WhereConditions);
            WhereConditions.Add(conditionGroup);
            return this;
        }

        public virtual OracleConditionsManager<T> And(Action<OracleConditionsManager<T>> orCallback)
        {
            FluentWhereConditionGroup conditionGroup = new()
            { CompareMethod = CompareMethods.And };
            OracleConditionsManager<T> manager = new();
            orCallback(manager);
            conditionGroup.Conditions.AddRange(manager.WhereConditions);
            WhereConditions.Add(conditionGroup);
            return this;
        }

        public virtual OracleConditionsManager<T> Raw<TProperty>(Expression<Func<T, TProperty>> property, string whereCondition)
        {
            WhereConditions.Add(OracleOp.Raw(property, whereCondition));
            return this;
        }

        public virtual OracleConditionsManager<T> Raw(string whereCondition)
        {
            WhereConditions.Add(new OracleRawCondition() { RawCondition = whereCondition });
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

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Between<TProperty>(Expression<Func<T, TProperty>> property, TProperty value1, TProperty value2) => Between(property, value1, value2);

        IFluentConditionsManager<T> IFluentConditionsManager<T>.Clone(IFluentConditionsManager<T> sourceIConditionsManager) => Clone((OracleConditionsManager<T>)sourceIConditionsManager);

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