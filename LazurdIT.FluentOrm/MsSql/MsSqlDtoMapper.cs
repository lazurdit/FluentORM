using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LazurdIT.FluentOrm.MsSql
{
    internal class MsSqlDtoMapper<T> : DtoMapper<T, SqlCommand> where T : IFluentModel, new()
    {
        public MsSqlDtoMapper(FluentTypeDictionary? typesRequired = null) : base(typesRequired)
        {
        }

        public string GetPrimaryKeySqlWhereString(string parameterName = "")
        {
            var parameters = new List<SqlParameter>();
            var result = typeCache.Where(t => t.Value.Attribute.IsPrimary == true);

            return string.Join("and", result.Select(t => $"{t.Value.FinalPropertyName} = @{parameterName}{t.Value.FinalPropertyName}"));
        }

        public IEnumerable<SqlParameter> GetPrimaryKeySqlParameters(T instance, string parameterName = "")
        {
            var parameters = new List<SqlParameter>();
            var result = typeCache.Where(t => t.Value.Attribute.IsPrimary == true);

            return result.Select(t => new SqlParameter($"@{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance)));
        }

        public IEnumerable<SqlParameter> GetSqlParameters(T instance, string parameterName = "", string[]? fieldNamesList = null)
        {
            var parameters = new List<SqlParameter>();
            var result = fieldNamesList != null ? typeCache.Where(t => fieldNamesList.Contains(t.Value.FinalPropertyName)) : typeCache;
            return result.Select(t => new SqlParameter($"@{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance)));
        }

        public override T? ToDtoModel(SqlCommand cmd, string paramPrefix = "") => throw new Exception("Method unsupported for MsSql");
    }
}