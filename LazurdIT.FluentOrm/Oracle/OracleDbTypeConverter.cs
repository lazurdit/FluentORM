using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace LazurdIT.FluentOrm.Oracle;

public static class OracleDbTypeConverter
{
    public static Type GetCSharpType(OracleDbType oracleDbType) => oracleDbType switch
    {
        OracleDbType.Varchar2 or OracleDbType.NVarchar2 or OracleDbType.Char or OracleDbType.NChar or OracleDbType.Clob or OracleDbType.NClob or OracleDbType.Long => typeof(string),
        OracleDbType.Blob or OracleDbType.Raw or OracleDbType.BFile or OracleDbType.LongRaw => typeof(byte[]),
        OracleDbType.Date or OracleDbType.TimeStamp => typeof(DateTime),
        OracleDbType.TimeStampLTZ or OracleDbType.TimeStampTZ => typeof(DateTimeOffset),
        OracleDbType.IntervalDS => typeof(TimeSpan),
        OracleDbType.IntervalYM => typeof(int),
        OracleDbType.Decimal => typeof(decimal),
        OracleDbType.Double => typeof(double),
        OracleDbType.Single => typeof(float),
        OracleDbType.Int16 => typeof(short),
        OracleDbType.Int32 => typeof(int),
        OracleDbType.Int64 => typeof(long),
        OracleDbType.Byte => typeof(byte),
        OracleDbType.Boolean => typeof(bool),
        OracleDbType.RefCursor => typeof(OracleDataReader),
        OracleDbType.XmlType or OracleDbType.Json or OracleDbType.ArrayAsJson or OracleDbType.ObjectAsJson => typeof(string),
        OracleDbType.Ref => typeof(object),
        OracleDbType.BinaryDouble => typeof(double),
        OracleDbType.BinaryFloat => typeof(float),
        OracleDbType.Vector or OracleDbType.Vector_Int8 => typeof(sbyte[]),
        OracleDbType.Vector_Float32 => typeof(float[]),
        OracleDbType.Vector_Float64 => typeof(double[]),
        OracleDbType.Object => typeof(object),
        OracleDbType.Array => typeof(Array),
        _ => throw new ArgumentOutOfRangeException(nameof(oracleDbType), $"Unsupported OracleDbType: {oracleDbType}"),
    };

    public static object? GetValue(OracleDbType dbType, object value, Type? targetType)
    {
        var result = GetValue(dbType, value);
        if (result == null)
            return null;
        if (targetType == null)
            return result;

        if ((targetType == typeof(DateTimeOffset) || targetType == typeof(DateTimeOffset?)) && (result.GetType() == typeof(DateTimeOffset) || result.GetType() == typeof(DateTimeOffset?)))
            return result;

        if (targetType == typeof(DateTimeOffset))
            return dbType switch
            {
                OracleDbType.TimeStampLTZ => new DateTimeOffset(((OracleTimeStampTZ)result).Value),
                OracleDbType.TimeStampTZ => new DateTimeOffset(((OracleTimeStampTZ)result).Value),
                _ => throw new ArgumentOutOfRangeException(nameof(dbType), $"Unsupported OracleDbType: {dbType}"),
            };

        if (targetType == typeof(DateTimeOffset?))
            return dbType switch
            {
                OracleDbType.TimeStampLTZ => new DateTimeOffset?(((OracleTimeStampTZ)result).Value),
                OracleDbType.TimeStampTZ => new DateTimeOffset?(((OracleTimeStampTZ)result).Value),
                _ => throw new ArgumentOutOfRangeException(nameof(dbType), $"Unsupported OracleDbType: {dbType}"),
            };

        if (Nullable.GetUnderlyingType(targetType) != null)
        {
            var underlyingType = Nullable.GetUnderlyingType(targetType);
            return result == null ? null : Convert.ChangeType(result, underlyingType!);
        }

        return result == null ? null : Convert.ChangeType(result, targetType!);
    }

    public static object? GetValue(OracleDbType dbType, object value)
    {
        if (string.IsNullOrWhiteSpace(value.ToString()!))
            return null;

        return dbType switch
        {
            OracleDbType.Varchar2 or OracleDbType.NVarchar2 or OracleDbType.Char or OracleDbType.NChar or OracleDbType.Clob or OracleDbType.NClob or OracleDbType.Long => value.ToString()!,
            OracleDbType.Blob or OracleDbType.Raw or OracleDbType.BFile or OracleDbType.LongRaw => Encoding.UTF8.GetBytes(value.ToString()!),
            OracleDbType.Date => ((OracleDate)value).Value,
            OracleDbType.TimeStamp => ((OracleTimeStamp)value).Value,
            OracleDbType.TimeStampLTZ => new DateTimeOffset(((OracleTimeStampTZ)value).Value),
            OracleDbType.TimeStampTZ => new DateTimeOffset(((OracleTimeStampTZ)value).Value),
            OracleDbType.IntervalDS => ((OracleIntervalDS)value).Value,
            OracleDbType.IntervalYM => ((OracleIntervalYM)value).Value,
            OracleDbType.Decimal => ((OracleDecimal)value).Value,
            OracleDbType.Boolean => ((OracleBoolean)value).Value,
            OracleDbType.RefCursor => throw new NotSupportedException(),
            OracleDbType.XmlType or OracleDbType.Json or OracleDbType.ArrayAsJson or OracleDbType.ObjectAsJson => value.ToString()!,
            OracleDbType.Ref => value.ToString()!,
            OracleDbType.Vector or OracleDbType.Vector_Int8 => Encoding.UTF8.GetBytes(value.ToString()!).Select(b => (sbyte)b).ToArray(),
            OracleDbType.Vector_Float32 => value.ToString()!.Split(',').Select(float.Parse).ToArray(),
            OracleDbType.Vector_Float64 => value.ToString()!.Split(',').Select(double.Parse).ToArray(),
            OracleDbType.Object => value.ToString()!,
            OracleDbType.Array => value.ToString()!,
            OracleDbType.Double => double.Parse(value.ToString()!),
            OracleDbType.Single => float.Parse(value.ToString()!),
            OracleDbType.Int16 => short.Parse(value.ToString()!),
            OracleDbType.Int32 => int.Parse(value.ToString()!),
            OracleDbType.Int64 => long.Parse(value.ToString()!),
            OracleDbType.Byte => byte.Parse(value.ToString()!),
            OracleDbType.BinaryDouble => double.Parse(value.ToString()!),
            OracleDbType.BinaryFloat => float.Parse(value.ToString()!),

            _ => throw new ArgumentOutOfRangeException(nameof(dbType), $"Unsupported OracleDbType: {dbType}"),
        };
    }

    public static OracleDbType GetDefaultDbType(Type csharpType) => (csharpType == typeof(string)) ? OracleDbType.Varchar2 :
        (csharpType == typeof(byte[])) ? OracleDbType.Blob :
        (csharpType == typeof(DateTime)) ? OracleDbType.Date :
        (csharpType == typeof(DateTimeOffset)) ? OracleDbType.TimeStampTZ :
        (csharpType == typeof(TimeSpan)) ? OracleDbType.IntervalDS :
        (csharpType == typeof(int)) ? OracleDbType.Int32 :
        (csharpType == typeof(short)) ? OracleDbType.Int16 :
        (csharpType == typeof(long)) ? OracleDbType.Int64 :
        (csharpType == typeof(decimal)) ? OracleDbType.Decimal :
        (csharpType == typeof(double)) ? OracleDbType.Double :
        (csharpType == typeof(float)) ? OracleDbType.Single :
        (csharpType == typeof(byte)) ? OracleDbType.Byte :
        (csharpType == typeof(bool)) ? OracleDbType.Boolean :
        (csharpType == typeof(OracleDataReader)) ? OracleDbType.RefCursor :
        (csharpType == typeof(sbyte[])) ? OracleDbType.Vector_Int8 :
        (csharpType == typeof(float[])) ? OracleDbType.Vector_Float32 :
        (csharpType == typeof(double[])) ? OracleDbType.Vector_Float64 :
        (csharpType == typeof(object)) ? OracleDbType.Object :
        (csharpType == typeof(Array)) ? OracleDbType.Array :

        (csharpType == typeof(byte?[])) ? OracleDbType.Blob :
        (csharpType == typeof(DateTime?)) ? OracleDbType.Date :
        (csharpType == typeof(DateTimeOffset?)) ? OracleDbType.TimeStampTZ :
        (csharpType == typeof(TimeSpan?)) ? OracleDbType.IntervalDS :
        (csharpType == typeof(int?)) ? OracleDbType.Int32 :
        (csharpType == typeof(short?)) ? OracleDbType.Int16 :
        (csharpType == typeof(long?)) ? OracleDbType.Int64 :
        (csharpType == typeof(decimal?)) ? OracleDbType.Decimal :
        (csharpType == typeof(double?)) ? OracleDbType.Double :
        (csharpType == typeof(float?)) ? OracleDbType.Single :
        (csharpType == typeof(byte?)) ? OracleDbType.Byte :
        (csharpType == typeof(bool?)) ? OracleDbType.Boolean :
        (csharpType == typeof(sbyte?[])) ? OracleDbType.Vector_Int8 :
        (csharpType == typeof(float?[])) ? OracleDbType.Vector_Float32 :
        (csharpType == typeof(double?[])) ? OracleDbType.Vector_Float64 :
        throw new ArgumentOutOfRangeException(nameof(csharpType), $"Unsupported C# type: {csharpType}");
}