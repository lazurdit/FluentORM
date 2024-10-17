using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Common
{
    internal static class TypeCache
    {
        internal static readonly Dictionary<Type, FluentTypeDictionary> _propertyCache =
               new();

        internal static FluentTypeDictionary GetTypeCache<T>() where T : IFluentModel
        {
            if (_propertyCache.TryGetValue(typeof(T), out FluentTypeDictionary? value))
                return value;
            else
            {
                var dict = new FluentTypeDictionary();
                var properties = typeof(T).GetProperties();

                foreach (var property in properties)
                {
                    var attribute = (FluentFieldAttribute?)Attribute.GetCustomAttribute(property, typeof(FluentFieldAttribute));
                    if (attribute != null)
                    {
                        dict[property.Name] = new(property, attribute);
                    }
                }
                _propertyCache[typeof(T)] = dict;
                return dict;
            }
        }

        internal static FluentTypeInfo? GetTypeInfo<T, TProperty>(Expression<Func<T, TProperty>> property) where T : IFluentModel
        {
            KeyValuePair<string, FluentTypeInfo>? prop = GetTypeCache<T>().FirstOrDefault(t => t.Key == property.Name);

            if (prop != null)
            {
                return prop.Value.Value;
            }

            return null;
        }
    }
}