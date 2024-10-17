using LazurdIT.FluentOrm.Common;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LazurdIT.FluentOrm.MySql
{
    internal class MySqlDtoMapper<T> : DtoMapper<T, MySqlCommand> where T : IFluentModel, new()
    {
        public MySqlDtoMapper(FluentTypeDictionary? typesRequired = null) : base(typesRequired)
        {
        }

        public string GetPrimaryKeySqlWhereString(string parameterName = "")
        {
            var parameters = new List<MySqlParameter>();
            var result = typeCache.Where(t => t.Value.Attribute.IsPrimary == true);

            return string.Join("and", result.Select(t => $"{t.Value.FinalPropertyName} = @{parameterName}{t.Value.FinalPropertyName}"));
        }

        public IEnumerable<MySqlParameter> GetPrimaryKeySqlParameters(T instance, string parameterName = "")
        {
            var parameters = new List<MySqlParameter>();
            var result = typeCache.Where(t => t.Value.Attribute.IsPrimary == true);

            return result.Select(t => new MySqlParameter($"@{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance)));
        }

        public IEnumerable<MySqlParameter> GetSqlParameters(T instance, string parameterName = "", string[]? fieldNamesList = null)
        {
            var parameters = new List<MySqlParameter>();
            var result = fieldNamesList != null ? typeCache.Where(t => fieldNamesList.Contains(t.Value.FinalPropertyName)) : typeCache;
            return result.Select(t => new MySqlParameter($"@{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance)));
        }

        public override T? ToDtoModel(MySqlCommand cmd, string paramPrefix = "")
        {
            var instance = new T();

            foreach (var (_, fluentTypeInfo) in typeCache)
            {
                var parameterName = $"{paramPrefix}{fluentTypeInfo.FinalPropertyName}";
                if (cmd.Parameters.Contains(parameterName) && cmd.Parameters[parameterName].Value != DBNull.Value)
                {
                    var val2 = MySqlDbTypeConverter.GetValue(cmd.Parameters[parameterName].MySqlDbType, cmd.Parameters[parameterName].Value!);
                    if (val2 != DBNull.Value && val2 != null)
                        fluentTypeInfo.Property.SetValue(instance, val2);
                }
            }

            return instance;
        }
    }
}