using MySqlConnector;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Data;
using System.Text;

namespace LazurdIT.FluentOrm.MySql;

public static class MySqlDbTypeConverter
{
    public static Type GetCSharpType(MySqlDbType mySqlDbType) => mySqlDbType switch
    {
        MySqlDbType.VarChar or MySqlDbType.Text or MySqlDbType.String or MySqlDbType.TinyText or MySqlDbType.MediumText or MySqlDbType.LongText or MySqlDbType.VarString => typeof(string),
        MySqlDbType.Blob or MySqlDbType.Binary or MySqlDbType.VarBinary or MySqlDbType.TinyBlob or MySqlDbType.MediumBlob or MySqlDbType.LongBlob => typeof(byte[]),
        MySqlDbType.Date or MySqlDbType.DateTime or MySqlDbType.Timestamp or MySqlDbType.Newdate => typeof(DateTime),
        MySqlDbType.Time => typeof(TimeSpan),
        MySqlDbType.Year => typeof(int),
        MySqlDbType.Decimal or MySqlDbType.NewDecimal => typeof(decimal),
        MySqlDbType.Double => typeof(double),
        MySqlDbType.Float => typeof(float),
        MySqlDbType.Int16 => typeof(short),
        MySqlDbType.Int32 or MySqlDbType.Int24 => typeof(int),
        MySqlDbType.Int64 => typeof(long),
        MySqlDbType.Byte or MySqlDbType.UByte => typeof(byte),
        MySqlDbType.Bit or MySqlDbType.Bool => typeof(bool),
        MySqlDbType.JSON => typeof(string),
        MySqlDbType.Enum => typeof(string),
        MySqlDbType.Set => typeof(string),
        MySqlDbType.Guid => typeof(Guid),
        MySqlDbType.Geometry => typeof(string),
        MySqlDbType.UInt16 => typeof(ushort),
        MySqlDbType.UInt32 or MySqlDbType.UInt24 => typeof(uint),
        MySqlDbType.UInt64 => typeof(ulong),
        MySqlDbType.Null => typeof(DBNull),

        _ => throw new ArgumentOutOfRangeException(nameof(mySqlDbType), $"Unsupported MySqlDbType: {mySqlDbType}"),
    };

    public static object? GetValue(MySqlDbType dbType, object value, Type? targetType)
    {
        var result = GetValue(dbType, value);
        if (result == null)
            return null;
        return targetType == null ? result : Convert.ChangeType(result, targetType!);
    }

    public static object? GetValue(MySqlDbType dbType, object value)
    {
        if (string.IsNullOrWhiteSpace(value.ToString()!))
            return null;

        return dbType switch
        {
            MySqlDbType.VarChar or MySqlDbType.VarString or MySqlDbType.Text or MySqlDbType.String or MySqlDbType.TinyText or MySqlDbType.MediumText or MySqlDbType.LongText => value.ToString()!,
            MySqlDbType.Blob or MySqlDbType.Binary or MySqlDbType.VarBinary or MySqlDbType.TinyBlob or MySqlDbType.MediumBlob or MySqlDbType.LongBlob => Encoding.UTF8.GetBytes(value.ToString()!),
            MySqlDbType.Date or MySqlDbType.DateTime or MySqlDbType.Timestamp or MySqlDbType.Newdate => DateTime.Parse(value.ToString()!),
            MySqlDbType.Time => TimeSpan.Parse(value.ToString()!),
            MySqlDbType.Year => int.Parse(value.ToString()!),
            MySqlDbType.Decimal or MySqlDbType.NewDecimal => decimal.Parse(value.ToString()!),
            MySqlDbType.Double => double.Parse(value.ToString()!),
            MySqlDbType.Float => float.Parse(value.ToString()!),
            MySqlDbType.Int16 or MySqlDbType.UInt16 => short.Parse(value.ToString()!),
            MySqlDbType.Int32 or MySqlDbType.Int24 or MySqlDbType.UInt32 or MySqlDbType.UInt24 => int.Parse(value.ToString()!),
            MySqlDbType.Int64 or MySqlDbType.UInt64 => long.Parse(value.ToString()!),
            MySqlDbType.Byte or MySqlDbType.UByte => byte.Parse(value.ToString()!),
            MySqlDbType.Bit or MySqlDbType.Bool => bool.Parse(value.ToString()!),
            MySqlDbType.JSON => value.ToString()!,
            MySqlDbType.Enum => value.ToString()!,
            MySqlDbType.Set => value.ToString()!,
            MySqlDbType.Guid => Guid.Parse(value.ToString()!),
            MySqlDbType.Geometry => value.ToString()!,
            MySqlDbType.Null => DBNull.Value,
            _ => throw new ArgumentOutOfRangeException(nameof(dbType), $"Unsupported MySqlDbType: {dbType}"),
        };
    }

    public static MySqlDbType GetDefaultDbType(Type csharpType) => csharpType == typeof(string) ? MySqlDbType.VarChar :
            csharpType == typeof(byte[]) ? MySqlDbType.Blob :
            csharpType == typeof(DateTime) ? MySqlDbType.DateTime :
            csharpType == typeof(decimal) ? MySqlDbType.Decimal :
            csharpType == typeof(double) ? MySqlDbType.Double :
            csharpType == typeof(float) ? MySqlDbType.Float :
            csharpType == typeof(int) ? MySqlDbType.Int32 :
            csharpType == typeof(short) ? MySqlDbType.Int16 :
            csharpType == typeof(long) ? MySqlDbType.Int64 :
            csharpType == typeof(byte) ? MySqlDbType.Byte :
            csharpType == typeof(bool) ? MySqlDbType.Bit :

            csharpType == typeof(byte?[]) ? MySqlDbType.Blob :
            csharpType == typeof(DateTime?) ? MySqlDbType.DateTime :
            csharpType == typeof(decimal?) ? MySqlDbType.Decimal :
            csharpType == typeof(double?) ? MySqlDbType.Double :
            csharpType == typeof(float?) ? MySqlDbType.Float :
            csharpType == typeof(int?) ? MySqlDbType.Int32 :
            csharpType == typeof(short?) ? MySqlDbType.Int16 :
            csharpType == typeof(long?) ? MySqlDbType.Int64 :
            csharpType == typeof(byte?) ? MySqlDbType.Byte :
            csharpType == typeof(bool?) ? MySqlDbType.Bit :

            throw new ArgumentOutOfRangeException(nameof(csharpType), $"Unsupported C# type: {csharpType}");
}