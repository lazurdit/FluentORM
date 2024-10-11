using System.Data;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Common;

public interface IDeleteQuery<T> : IConditionQuery<T>, ITableRelatedFluentQuery
    where T : IFluentModel, new()
{
    int Execute(DbConnection? connection = null, bool deleteAll = false);

    IDeleteQuery<T> Where(Action<IConditionsManager<T>> fn);
}