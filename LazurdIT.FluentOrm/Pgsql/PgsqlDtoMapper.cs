using LazurdIT.FluentOrm.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LazurdIT.FluentOrm.Pgsql
{
    internal class PgsqlDtoMapper<T> : DtoMapper<T, NpgsqlCommand> where T : IFluentModel, new()
    {
        public PgsqlDtoMapper(FluentTypeDictionary? typesRequired = null) : base(typesRequired)
        {
        }

        public string GetPrimaryKeySqlWhereString(string parameterName = "")
        {
            var parameters = new List<NpgsqlParameter>();
            var result = typeCache.Where(t => t.Value.Attribute.IsPrimary == true);

            return string.Join("and", result.Select(t => $"{t.Value.FinalPropertyName} = @{parameterName}{t.Value.FinalPropertyName}"));
        }

        public IEnumerable<NpgsqlParameter> GetPrimaryKeySqlParameters(T instance, string parameterName = "")
        {
            var parameters = new List<NpgsqlParameter>();
            var result = typeCache.Where(t => t.Value.Attribute.IsPrimary == true);

            return result.Select(t => new NpgsqlParameter($"@{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance) ?? DBNull.Value));
        }

        public IEnumerable<NpgsqlParameter> GetSqlParameters(T instance, string parameterName = "", string[]? fieldNamesList = null)
        {
            var parameters = new List<NpgsqlParameter>();
            var result = fieldNamesList != null ? typeCache.Where(t => fieldNamesList.Contains(t.Value.FinalPropertyName)) : typeCache;
            return result.Select(t => new NpgsqlParameter($"@{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance) ?? DBNull.Value));
        }

        public override T? ToDtoModel(NpgsqlCommand cmd, string paramPrefix = "") => throw new Exception("Method unsupported for PgSql");
    }
}