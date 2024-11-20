using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazurdIT.FluentOrm.Common
{
    public class MySqlFluentUpdateCriteriaManager<T> : FluentUpdateCriteriaManager<T> where T : IFluentModel, new()
    {
        public IEnumerable<MySqlParameter> GetSqlParameters(T? instance, string parameterName)
        {
            return Criterias.Where(t => t.Value?.Details?.HasParameter ?? false).Select(t => new MySqlParameter($"@{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance) ?? DBNull.Value));
        }
    }
}