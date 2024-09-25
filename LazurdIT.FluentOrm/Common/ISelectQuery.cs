using System.Data.Common;

namespace LazurdIT.FluentOrm.Common;

public interface ISelectQuery<T> : IConditionQuery<T> where T : IFluentModel, new()
{
    IFieldsSelectionManager<T> FieldsManager { get; }
    OrderByManager<T> OrderByManager { get; }
    DbConnection? Connection { get; }
    string TableName { get; set; }

    IEnumerable<T> Execute(DbConnection? sqlConnection = null, int pageNumber = 0, int recordsCount = 0);

    ISelectQuery<T> OrderBy(Action<OrderByManager<T>> fn);

    ISelectQuery<T> Returns(Action<IFieldsSelectionManager<T>> fn);

    ISelectQuery<T> Where(Action<IConditionsManager<T>> fn);
}