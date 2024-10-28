using System;

namespace LazurdIT.FluentOrm.Common
{
    public class DbTypeComverter
    {
        public static object? ReverseGetValue(object value, Type? targetType)
        {
            var result = value;
            if (result == null)
                return null;
            else if (targetType == null)
                return result;
            else if (Nullable.GetUnderlyingType(targetType) != null && Nullable.GetUnderlyingType(targetType) == typeof(long) && result.GetType() == typeof(decimal) && long.TryParse($"{result}", out long longResult2))
                return longResult2;
            if (targetType == typeof(long) && result.GetType() == typeof(decimal) && long.TryParse($"{result}", out long longResult))
                return longResult;
            else if ((targetType == typeof(DateTimeOffset) || targetType == typeof(DateTimeOffset?)) && (result.GetType() == typeof(DateTimeOffset) || result.GetType() == typeof(DateTimeOffset?)))
                return result;
            else if (targetType == typeof(DateTimeOffset) && value is DateTime dateTimeValue)
                return new DateTimeOffset(dateTimeValue);
            else if (targetType == typeof(DateTimeOffset?) && value is DateTime dateTimeValue2)
                return new DateTimeOffset(dateTimeValue2);
            else if (Nullable.GetUnderlyingType(targetType) != null)
                return result == null ? null : Convert.ChangeType(result, Nullable.GetUnderlyingType(targetType)!);
            else
                return result == null ? null : Convert.ChangeType(result, targetType!);
        }
    }
}