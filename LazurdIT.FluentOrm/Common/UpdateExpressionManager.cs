using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Common
{
    public class FluentUpdateCriteriaManager<T> where T : IFluentModel, new()
    {
        public FluentTypeDictionary<IUpdateExpression> Criterias { get; private set; }
        private readonly FluentTypeDictionary originalFields;

        public virtual IEnumerable<string> GetFinalExpressions(string parameterName, string expressionSymbol) => Criterias.Where(c => c.Value?.Details != null).Select(c => c.Value.Details!.GetExpression(parameterName, expressionSymbol));

        public List<string> GetFinalPropertyNames() => Criterias.GetFinalPropertyNames();

        public FluentUpdateCriteriaManager()
        {
            Criterias = new();
            originalFields = new(TypeCache.GetTypeCache<T>());
            IncludeAll();
        }

        public virtual FluentUpdateCriteriaManager<T> FromField<TProperty>(Expression<Func<T, TProperty>> property)
        {
            string propertyName = AttributeResolver.ResolvePropertyName(property);
            if (originalFields.TryGetValue(propertyName, out var originalProperty))
            {
                IUpdateExpression updateExpression = new ItemUpdateExpression(originalProperty.FinalPropertyName);
                FluentTypeInfo<IUpdateExpression> info = new(originalProperty.Property, originalProperty.Attribute, updateExpression);
                if (Criterias.ContainsKey(propertyName))
                    Criterias[propertyName] = info;
                else
                    Criterias.Add(propertyName, info);
                return this;
            }

            throw new Exception($"Property {propertyName} not found in the object fields");
        }

        public virtual FluentUpdateCriteriaManager<T> ExcludeAll()
        {
            Criterias = new();
            return this;
        }

        public virtual FluentUpdateCriteriaManager<T> Exclude<TProperty>(Expression<Func<T, TProperty>> property)
        {
            string propertyName = AttributeResolver.ResolvePropertyName(property);
            if (originalFields.TryGetValue(propertyName, out var originalProperty))
            {
                if (Criterias.ContainsKey(originalProperty.FinalPropertyName))
                    Criterias.Remove(originalProperty.FinalPropertyName);
            }
            return this;
        }

        public virtual FluentUpdateCriteriaManager<T> IncludeAll()
        {
            foreach (var value in originalFields.Values)
            {
                string propertyName = value.FinalPropertyName;

                IUpdateExpression updateExpression = new ItemUpdateExpression(value.FinalPropertyName);

                FluentTypeInfo<IUpdateExpression> info = new(value.Property, value.Attribute, updateExpression);
                if (Criterias.ContainsKey(propertyName))
                    Criterias[propertyName] = info;
                else
                    Criterias.Add(propertyName, info);
            }

            return this;
        }

        public virtual FluentUpdateCriteriaManager<T> FromFieldExpression<TProperty>(Expression<Func<T, TProperty>> property, string expression, string replacement = "%")
        {
            string propertyName = AttributeResolver.ResolvePropertyName(property);

            if (originalFields.TryGetValue(propertyName, out var originalProperty))
            {
                IUpdateExpression updateExpression = new CustomUpdateExpression(originalProperty.FinalPropertyName, expression, replacement);
                FluentTypeInfo<IUpdateExpression> info = new(originalProperty.Property, originalProperty.Attribute, updateExpression);
                if (Criterias.ContainsKey(propertyName))
                    Criterias[propertyName] = info;
                else
                    Criterias.Add(propertyName, info);
                return this;
            }
            throw new Exception($"Property {propertyName} not found in the object fields");
        }
    }
}