using System.Data;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Common;

public interface IDeleteQuery<T> : IConditionQuery<T> where T : IFluentModel, new()
{
    IDbConnection? Connection { get; }

    string TableName { get; set; }

    int Execute(DbConnection? sqlConnection = null, bool deleteAll = false);

    IDeleteQuery<T> Where(Action<IConditionsManager<T>> fn);
}