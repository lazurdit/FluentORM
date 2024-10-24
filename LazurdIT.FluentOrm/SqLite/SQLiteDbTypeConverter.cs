using System;
using System.Data;
using System.Text;

namespace LazurdIT.FluentOrm.SQLite
{
    public static class SQLiteDbTypeConverter
    {
        public static Type GetCSharpType(DbType dbType) => dbType switch
        {
            DbType.String => typeof(string),
            DbType.Binary => typeof(byte[]),
            DbType.Date => typeof(DateTime),
            DbType.DateTime => typeof(DateTime),
            DbType.Time => typeof(TimeSpan),
            DbType.Decimal => typeof(decimal),
            DbType.Double => typeof(double),
            DbType.Single => typeof(float),
            DbType.Int16 => typeof(short),
            DbType.Int32 => typeof(int),
            DbType.Int64 => typeof(long),
            DbType.Byte => typeof(byte),
            DbType.Boolean => typeof(bool),
            DbType.Guid => typeof(Guid),
            DbType.Object => typeof(object),
            _ => throw new ArgumentOutOfRangeException(nameof(dbType), $"Unsupported DbType: {dbType}")
        };

        public static object? GetValue(DbType dbType, object value, Type? targetType)
        {
            var result = GetValue(dbType, value);
            if (result == null)
                return null;
            return targetType == null ? result : Convert.ChangeType(result, targetType!);
        }

        public static object? GetValue(DbType dbType, object value)
        {
            if (string.IsNullOrWhiteSpace(value.ToString()!))
                return null;

            return dbType switch
            {
                DbType.String => value.ToString()!,
                DbType.Binary => Encoding.UTF8.GetBytes(value.ToString()!),
                DbType.Date => DateTime.Parse(value.ToString()!),
                DbType.DateTime => DateTime.Parse(value.ToString()!),
                DbType.Time => TimeSpan.Parse(value.ToString()!),
                DbType.Decimal => decimal.Parse(value.ToString()!),
                DbType.Double => double.Parse(value.ToString()!),
                DbType.Single => float.Parse(value.ToString()!),
                DbType.Int16 => short.Parse(value.ToString()!),
                DbType.Int32 => int.Parse(value.ToString()!),
                DbType.Int64 => long.Parse(value.ToString()!),
                DbType.Byte => byte.Parse(value.ToString()!),
                DbType.Boolean => bool.Parse(value.ToString()!),
                DbType.Guid => Guid.Parse(value.ToString()!),
                DbType.Object => value,
                _ => throw new ArgumentOutOfRangeException(nameof(dbType), $"Unsupported DbType: {dbType}")
            };
        }

        public static DbType GetDefaultDbType(Type csharpType) => csharpType == typeof(string) ? DbType.String :
                csharpType == typeof(byte[]) ? DbType.Binary :
                csharpType == typeof(DateTime) ? DbType.DateTime :
                csharpType == typeof(decimal) ? DbType.Decimal :
                csharpType == typeof(double) ? DbType.Double :
                csharpType == typeof(float) ? DbType.Single :
                csharpType == typeof(int) ? DbType.Int32 :
                csharpType == typeof(short) ? DbType.Int16 :
                csharpType == typeof(long) ? DbType.Int64 :
                csharpType == typeof(byte) ? DbType.Byte :
                csharpType == typeof(bool) ? DbType.Boolean :
                csharpType == typeof(Guid) ? DbType.Guid :

                csharpType == typeof(byte?[]) ? DbType.Binary :
                csharpType == typeof(DateTime?) ? DbType.DateTime :
                csharpType == typeof(decimal?) ? DbType.Decimal :
                csharpType == typeof(double?) ? DbType.Double :
                csharpType == typeof(float?) ? DbType.Single :
                csharpType == typeof(int?) ? DbType.Int32 :
                csharpType == typeof(short?) ? DbType.Int16 :
                csharpType == typeof(long?) ? DbType.Int64 :
                csharpType == typeof(byte?) ? DbType.Byte :
                csharpType == typeof(bool?) ? DbType.Boolean :

                throw new ArgumentOutOfRangeException(nameof(csharpType), $"Unsupported C# type: {csharpType}");
    }
}