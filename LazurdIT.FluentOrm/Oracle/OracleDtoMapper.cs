using System.Data;
using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;

namespace LazurdIT.FluentOrm.Oracle;

internal class OracleDtoMapper<T> : DtoMapper<T, OracleCommand> where T : IFluentModel, new()
{
    public OracleDtoMapper(FluentTypeDictionary? typesRequired = null) : base(typesRequired)
    {
    }

    public string GetPrimaryKeySqlWhereString(string parameterName = "")
    {
        var parameters = new List<OracleParameter>();
        var result = typeCache.Where(t => t.Value.Attribute.IsPrimary == true);

        return string.Join("and", result.Select(t => $"{t.Value.FinalPropertyName} = :{parameterName}{t.Value.FinalPropertyName}"));
    }

    public IEnumerable<OracleParameter> GetPrimaryKeySqlParameters(T instance, string parameterName = "")
    {
        var parameters = new List<OracleParameter>();
        var result = typeCache.Where(t => t.Value.Attribute.IsPrimary == true);

        return result.Select(t => new OracleParameter($":{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance)));
    }

    public IEnumerable<OracleParameter> GetSqlParameters(T instance, string parameterName = "", string[]? fieldNamesList = null)
    {
        var parameters = new List<OracleParameter>();
        var result = fieldNamesList != null ? typeCache.Where(t => fieldNamesList.Contains(t.Value.FinalPropertyName)) : typeCache;
        return result.Select(t => new OracleParameter($":{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance)));
    }

    public override T? ToDtoModel(OracleCommand cmd, string paramPrefix = "")
    {
        var instance = new T();

        foreach (var (_, fluentTypeInfo) in typeCache)
        {
            var parameterName = $"{paramPrefix}{fluentTypeInfo.FinalPropertyName}";
            if (cmd.Parameters.Contains(parameterName) && cmd.Parameters[parameterName].Value != DBNull.Value)
            {
                var val2 = OracleDbTypeConverter.GetValue(cmd.Parameters[parameterName].OracleDbType, cmd.Parameters[parameterName].Value, fluentTypeInfo.Property.PropertyType);
                if (val2 != DBNull.Value && val2 != null)
                    fluentTypeInfo.Property.SetValue(instance, val2);
            }
        }

        return instance;
    }
}