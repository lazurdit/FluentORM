using System;
using System.Reflection;

namespace LazurdIT.FluentOrm.Common
{
    public class FluentTypeInfo<T>
    {
        public T? Details { get; }
        public PropertyInfo Property { get; }
        public FluentFieldAttribute Attribute { get; }
        public string FinalPropertyName => Attribute.Name ?? Property.Name;

        public FluentTypeInfo(PropertyInfo property, FluentFieldAttribute attribute, T? details)
        {
            Details = details;
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
        }
    }
}