using System;
using System.Reflection;

namespace LazurdIT.FluentOrm.Common
{
    public class FluentTypeInfo
    {
        public FluentTypeInfo(PropertyInfo property, FluentFieldAttribute attribute)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
            Attribute = attribute ?? throw new ArgumentNullException(nameof(attribute));
        }

        public PropertyInfo Property { get; }
        public FluentFieldAttribute Attribute { get; }

        public object? ParseValue(object value)
        {
            if (value == null) return null;

            try
            {
                var targetType = Property.PropertyType;

                if (Nullable.GetUnderlyingType(targetType) != null)
                    targetType = Nullable.GetUnderlyingType(targetType);

                if (targetType == null)
                    return null;

                return Convert.ChangeType(value, targetType);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to convert value to {Property.PropertyType}", ex);
            }
        }

        public string FinalPropertyName => Attribute.Name ?? Property.Name;
    }
}