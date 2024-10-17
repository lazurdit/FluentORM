using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Linq;

namespace LazurdIT.FluentOrm.Common
{
    public class OracleFluentUpdateCriteriaManager<T> : FluentUpdateCriteriaManager<T> where T : IFluentModel, new()
    {
        public override IEnumerable<string> GetFinalExpressions(string parameterName, string expressionSymbol)
        {
            return base.GetFinalExpressions(parameterName, expressionSymbol);
        }

        public IEnumerable<OracleParameter> GetSqlParameters(T? instance, string parameterName)
        {
            return Criterias.Where(t => t.Value?.Details?.HasParameter ?? false).Select(t => new OracleParameter($"{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance)));
        }
    }
}