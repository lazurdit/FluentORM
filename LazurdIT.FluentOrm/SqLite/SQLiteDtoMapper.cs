using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;

namespace LazurdIT.FluentOrm.SQLite
{
    internal class SQLiteDtoMapper<T> : DtoMapper<T, SQLiteCommand> where T : IFluentModel, new()
    {
        public SQLiteDtoMapper(FluentTypeDictionary? typesRequired = null) : base(typesRequired)
        {
        }

        public string GetPrimaryKeySqlWhereString(string parameterName = "")
        {
            var parameters = new List<SQLiteParameter>();
            var result = typeCache.Where(t => t.Value.Attribute.IsPrimary == true);

            return string.Join("and", result.Select(t => $"{t.Value.FinalPropertyName} = @{parameterName}{t.Value.FinalPropertyName}"));
        }

        public IEnumerable<SQLiteParameter> GetPrimaryKeySqlParameters(T instance, string parameterName = "")
        {
            var parameters = new List<SQLiteParameter>();
            var result = typeCache.Where(t => t.Value.Attribute.IsPrimary == true);

            return result.Select(t => new SQLiteParameter($"@{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance)));
        }

        public IEnumerable<SQLiteParameter> GetSqlParameters(T instance, string parameterName = "", string[]? fieldNamesList = null)
        {
            var parameters = new List<SQLiteParameter>();
            var result = fieldNamesList != null ? typeCache.Where(t => fieldNamesList.Contains(t.Value.FinalPropertyName)) : typeCache;
            return result.Select(t => new SQLiteParameter($"@{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance)));
        }

        public override T? ToDtoModel(SQLiteCommand cmd, string paramPrefix = "") => throw new Exception("Method unsupported for SQLite");
    }
}