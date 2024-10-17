using System;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Common
{
    public class AggregateFieldsSelectionManager<T> where T : IFluentModel, new()
    {
        public FluentAggregateTypeDictionary FieldsList { get; private set; }
        private readonly FluentTypeDictionary originalFields;

        public AggregateFieldsSelectionManager()
        {
            FieldsList = new();
            originalFields = new(TypeCache.GetTypeCache<T>());
        }

        public AggregateFieldsSelectionManager<T> Custom<TProperty>(Expression<Func<T, TProperty>> property, string aggregationMethod, string? alias = null)
        {
            var field = Ag.ForField(property, alias, AggregationMethods.Custom, aggregationMethod);
            FieldsList.Add(field.FinalAlias, field);
            return this;
        }

        public virtual AggregateFieldsSelectionManager<T> Sum<TProperty>(Expression<Func<T, TProperty>> property, string? alias = null)
        {
            var field = Ag.SumForField(property, alias);
            FieldsList.Add(field.FinalAlias, field);
            return this;
        }

        public virtual AggregateFieldsSelectionManager<T> Count<TProperty>(Expression<Func<T, TProperty>> property, string? alias = null)
        {
            var field = Ag.CountForField(property, alias);
            FieldsList.Add(field.FinalAlias, field);
            return this;
        }

        public virtual AggregateFieldsSelectionManager<T> Avg<TProperty>(Expression<Func<T, TProperty>> property, string? alias = null)
        {
            var field = Ag.AvgForField(property, alias);
            FieldsList.Add(field.FinalAlias, field);
            return this;
        }

        public virtual AggregateFieldsSelectionManager<T> Min<TProperty>(Expression<Func<T, TProperty>> property, string? alias = null)
        {
            var field = Ag.MinForField(property, alias);
            FieldsList.Add(field.FinalAlias, field);
            return this;
        }

        public virtual AggregateFieldsSelectionManager<T> Max<TProperty>(Expression<Func<T, TProperty>> property, string? alias = null)
        {
            var field = Ag.MaxForField(property, alias);
            FieldsList.Add(field.FinalAlias, field);
            return this;
        }
    }
}