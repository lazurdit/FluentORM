using NpgsqlTypes;
using System;
using System.Text;

namespace LazurdIT.FluentOrm.Pgsql
{
    public static class NpgsqlDbTypeConverter
    {
        public static Type GetCSharpType(NpgsqlDbType npgsqlDbType) => npgsqlDbType switch
        {
            NpgsqlDbType.Varchar or NpgsqlDbType.Text or NpgsqlDbType.Char or NpgsqlDbType.Name or NpgsqlDbType.Json or NpgsqlDbType.Xml => typeof(string),
            NpgsqlDbType.Bytea => typeof(byte[]),
            NpgsqlDbType.Date or NpgsqlDbType.Timestamp or NpgsqlDbType.TimestampTz => typeof(DateTime),
            NpgsqlDbType.Time or NpgsqlDbType.TimeTz => typeof(TimeSpan),
            NpgsqlDbType.Interval => typeof(TimeSpan),
            NpgsqlDbType.Double => typeof(double),
            NpgsqlDbType.Real => typeof(float),
            NpgsqlDbType.Smallint => typeof(short),
            NpgsqlDbType.Integer => typeof(int),
            NpgsqlDbType.Bigint => typeof(long),
            NpgsqlDbType.Boolean => typeof(bool),
            NpgsqlDbType.Uuid => typeof(Guid),
            NpgsqlDbType.Jsonb => typeof(string),
            NpgsqlDbType.Hstore => typeof(string),
            NpgsqlDbType.Geometry => typeof(string),
            NpgsqlDbType.Numeric => typeof(decimal),
            NpgsqlDbType.Money => typeof(decimal),
            NpgsqlDbType.Citext => typeof(string),
            NpgsqlDbType.InternalChar => typeof(string),
            NpgsqlDbType.Inet => typeof(string),
            NpgsqlDbType.Cidr => typeof(string),
            NpgsqlDbType.MacAddr => typeof(string),
            NpgsqlDbType.MacAddr8 => typeof(string),

            _ => throw new ArgumentOutOfRangeException(nameof(npgsqlDbType), $"Unsupported NpgsqlDbType: {npgsqlDbType}"),
        };

        public static object? GetValue(NpgsqlDbType dbType, object value, Type? targetType)
        {
            var result = GetValue(dbType, value);
            if (result == null)
                return null;
            return targetType == null ? result : Convert.ChangeType(result, targetType!);
        }

        public static object? GetValue(NpgsqlDbType dbType, object value)
        {
            if (string.IsNullOrWhiteSpace(value.ToString()!))
                return null;

            return dbType switch
            {
                NpgsqlDbType.Varchar or NpgsqlDbType.Text or NpgsqlDbType.Char or NpgsqlDbType.Name or NpgsqlDbType.Json or NpgsqlDbType.Xml => value.ToString()!,
                NpgsqlDbType.Bytea => Encoding.UTF8.GetBytes(value.ToString()!),
                NpgsqlDbType.Date or NpgsqlDbType.Timestamp or NpgsqlDbType.TimestampTz => DateTime.Parse(value.ToString()!),
                NpgsqlDbType.Time or NpgsqlDbType.TimeTz => TimeSpan.Parse(value.ToString()!),
                NpgsqlDbType.Interval => TimeSpan.Parse(value.ToString()!),
                NpgsqlDbType.Double => double.Parse(value.ToString()!),
                NpgsqlDbType.Real => float.Parse(value.ToString()!),
                NpgsqlDbType.Smallint => short.Parse(value.ToString()!),
                NpgsqlDbType.Integer => int.Parse(value.ToString()!),
                NpgsqlDbType.Bigint => long.Parse(value.ToString()!),
                NpgsqlDbType.Boolean => bool.Parse(value.ToString()!),
                NpgsqlDbType.Uuid => Guid.Parse(value.ToString()!),
                NpgsqlDbType.Jsonb => value.ToString()!,
                NpgsqlDbType.Hstore => value.ToString()!,
                NpgsqlDbType.Geometry => value.ToString()!,
                _ => throw new ArgumentOutOfRangeException(nameof(dbType), $"Unsupported NpgsqlDbType: {dbType}"),
            };
        }

        public static NpgsqlDbType GetDefaultDbType(Type csharpType) => csharpType == typeof(string) ? NpgsqlDbType.Varchar :
                csharpType == typeof(byte[]) ? NpgsqlDbType.Bytea :
                csharpType == typeof(DateTime) ? NpgsqlDbType.Timestamp :
                csharpType == typeof(double) ? NpgsqlDbType.Double :
                csharpType == typeof(float) ? NpgsqlDbType.Real :
                csharpType == typeof(int) ? NpgsqlDbType.Integer :
                csharpType == typeof(short) ? NpgsqlDbType.Smallint :
                csharpType == typeof(long) ? NpgsqlDbType.Bigint :
                csharpType == typeof(bool) ? NpgsqlDbType.Boolean :
                csharpType == typeof(Guid) ? NpgsqlDbType.Uuid :
                csharpType == typeof(TimeSpan) ? NpgsqlDbType.Interval :

                throw new ArgumentOutOfRangeException(nameof(csharpType), $"Unsupported C# type: {csharpType}");
    }
}