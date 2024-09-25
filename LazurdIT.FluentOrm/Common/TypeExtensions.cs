namespace LazurdIT.FluentOrm.Common;

public class TypeExtensions
{
    public static bool IsNumeric(Type type)
    {
        var valueType = type;

        bool isNumericType = valueType == typeof(sbyte) ||
                             valueType == typeof(byte) ||
                             valueType == typeof(short) ||
                             valueType == typeof(ushort) ||
                             valueType == typeof(int) ||
                             valueType == typeof(uint) ||
                             valueType == typeof(long) ||
                             valueType == typeof(ulong) ||
                             valueType == typeof(nint) ||
                             valueType == typeof(nuint) ||
                             valueType == typeof(Half) ||
                             valueType == typeof(float) ||
                             valueType == typeof(double) ||
                             valueType == typeof(decimal);

        return isNumericType;
    }
}