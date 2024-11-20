using LazurdIT.FluentOrm.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlFluentUpdateCriteriaManager<T> : FluentUpdateCriteriaManager<T> where T : IFluentModel, new()
    {
        public IEnumerable<NpgsqlParameter> GetSqlParameters(T? instance, string parameterName)
        {
            return Criterias.Where(t => t.Value?.Details?.HasParameter ?? false).Select(t => new NpgsqlParameter($"@{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance) ?? DBNull.Value));
        }
    }
}