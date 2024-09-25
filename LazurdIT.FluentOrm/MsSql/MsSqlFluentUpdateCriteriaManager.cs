using System.Data.SqlClient;

namespace LazurdIT.FluentOrm.Common;

public class MsSqlFluentUpdateCriteriaManager<T> : FluentUpdateCriteriaManager<T> where T : IFluentModel, new()
{
    public IEnumerable<SqlParameter> GetSqlParameters(T? instance, string parameterName)
    {
        return Criterias.Where(t => t.Value?.Details?.HasParameter ?? false).Select(t => new SqlParameter($"@{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance)));
    }
}