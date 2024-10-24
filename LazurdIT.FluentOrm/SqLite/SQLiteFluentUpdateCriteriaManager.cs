using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;

namespace LazurdIT.FluentOrm.Common
{
    public class SQLiteFluentUpdateCriteriaManager<T> : FluentUpdateCriteriaManager<T> where T : IFluentModel, new()
    {
        public IEnumerable<SQLiteParameter> GetSqlParameters(T? instance, string parameterName)
        {
            return Criterias.Where(t => t.Value?.Details?.HasParameter ?? false).Select(t => new SQLiteParameter($"@{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance)));
        }
    }
}