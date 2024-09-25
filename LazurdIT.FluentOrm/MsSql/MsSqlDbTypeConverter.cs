using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Text;

namespace LazurdIT.FluentOrm.MsSql;

public static class SqlDbTypeConverter
{
    public static Type GetCSharpType(SqlDbType sqlDbType) => sqlDbType switch
    {
        SqlDbType.VarChar or SqlDbType.NVarChar or SqlDbType.Text or SqlDbType.NText or SqlDbType.Char or SqlDbType.NChar or SqlDbType.Xml => typeof(string),
        SqlDbType.Binary or SqlDbType.VarBinary or SqlDbType.Image => typeof(byte[]),
        SqlDbType.Date or SqlDbType.DateTime or SqlDbType.DateTime2 or SqlDbType.SmallDateTime or SqlDbType.DateTimeOffset => typeof(DateTime),
        SqlDbType.Time or SqlDbType.Timestamp => typeof(TimeSpan),
        SqlDbType.Decimal or SqlDbType.Money or SqlDbType.SmallMoney => typeof(decimal),
        SqlDbType.Float => typeof(double),
        SqlDbType.Real => typeof(float),
        SqlDbType.SmallInt => typeof(short),
        SqlDbType.Int => typeof(int),
        SqlDbType.BigInt => typeof(long),
        SqlDbType.TinyInt => typeof(byte),
        SqlDbType.Bit => typeof(bool),
        SqlDbType.UniqueIdentifier => typeof(Guid),
        SqlDbType.Variant or SqlDbType.Udt or SqlDbType.Structured or SqlDbType.Xml => typeof(object),
        _ => throw new ArgumentOutOfRangeException(nameof(sqlDbType), $"Unsupported SqlDbType: {sqlDbType}"),
    };

    public static object? GetValue(SqlDbType dbType, object value, Type? targetType)
    {
        var result = GetValue(dbType, value);
        if (result == null)
            return null;
        return targetType == null ? result : Convert.ChangeType(result, targetType!);
    }

    public static object? GetValue(SqlDbType dbType, object value)
    {
        if (string.IsNullOrWhiteSpace(value.ToString()!))
            return null;

        return dbType switch
        {
            SqlDbType.VarChar or SqlDbType.NVarChar or SqlDbType.Text or SqlDbType.NText or SqlDbType.Char or SqlDbType.NChar or SqlDbType.Xml => value.ToString()!,
            SqlDbType.Binary or SqlDbType.VarBinary or SqlDbType.Image => Encoding.UTF8.GetBytes(value.ToString()!),
            SqlDbType.Date or SqlDbType.DateTime or SqlDbType.DateTime2 or SqlDbType.SmallDateTime or SqlDbType.DateTimeOffset => DateTime.Parse(value.ToString()!),
            SqlDbType.Time or SqlDbType.Timestamp => TimeSpan.Parse(value.ToString()!),
            SqlDbType.Decimal or SqlDbType.Money or SqlDbType.SmallMoney => decimal.Parse(value.ToString()!),
            SqlDbType.Float => double.Parse(value.ToString()!),
            SqlDbType.Real => float.Parse(value.ToString()!),
            SqlDbType.SmallInt => short.Parse(value.ToString()!),
            SqlDbType.Int => int.Parse(value.ToString()!),
            SqlDbType.BigInt => long.Parse(value.ToString()!),
            SqlDbType.TinyInt => byte.Parse(value.ToString()!),
            SqlDbType.Bit => bool.Parse(value.ToString()!),
            SqlDbType.UniqueIdentifier => Guid.Parse(value.ToString()!),
            SqlDbType.Variant or SqlDbType.Udt or SqlDbType.Structured => value,

            _ => throw new ArgumentOutOfRangeException(nameof(dbType), $"Unsupported SqlDbType: {dbType}"),
        };
    }

    public static SqlDbType GetDefaultDbType(Type csharpType) => csharpType == typeof(string) ? SqlDbType.VarChar :
            csharpType == typeof(byte[]) ? SqlDbType.VarBinary :
            csharpType == typeof(DateTime) ? SqlDbType.DateTime :
            csharpType == typeof(decimal) ? SqlDbType.Decimal :
            csharpType == typeof(double) ? SqlDbType.Float :
            csharpType == typeof(float) ? SqlDbType.Real :
            csharpType == typeof(int) ? SqlDbType.Int :
            csharpType == typeof(short) ? SqlDbType.SmallInt :
            csharpType == typeof(long) ? SqlDbType.BigInt :
            csharpType == typeof(byte) ? SqlDbType.TinyInt :
            csharpType == typeof(bool) ? SqlDbType.Bit :
            csharpType == typeof(Guid) ? SqlDbType.UniqueIdentifier :

            csharpType == typeof(byte?[]) ? SqlDbType.VarBinary :
            csharpType == typeof(DateTime?) ? SqlDbType.DateTime :
            csharpType == typeof(decimal?) ? SqlDbType.Decimal :
            csharpType == typeof(double?) ? SqlDbType.Float :
            csharpType == typeof(float?) ? SqlDbType.Real :
            csharpType == typeof(int?) ? SqlDbType.Int :
            csharpType == typeof(short?) ? SqlDbType.SmallInt :
            csharpType == typeof(long?) ? SqlDbType.BigInt :
            csharpType == typeof(byte?) ? SqlDbType.TinyInt :
            csharpType == typeof(bool?) ? SqlDbType.Bit :

            throw new ArgumentOutOfRangeException(nameof(csharpType), $"Unsupported C# type: {csharpType}");
}