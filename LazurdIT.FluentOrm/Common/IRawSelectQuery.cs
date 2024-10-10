using System.Data.Common;

namespace LazurdIT.FluentOrm.Common;

public interface IRawSelectQuery<T> : IConditionQuery<T>
    where T : IFluentModel, new()
{
    IFieldsSelectionManager<T> FieldsManager { get; }
    OrderByManager<T> OrderByManager { get; }
    string SelectClause { get; set; }

    IEnumerable<T> Execute(
        DbConnection? connection = null,
        int pageNumber = 0,
        int recordsCount = 0
    );

    IRawSelectQuery<T> OrderBy(Action<OrderByManager<T>> fn);

    IRawSelectQuery<T> Returns(Action<IFieldsSelectionManager<T>> fn);

    IRawSelectQuery<T> Where(Action<IConditionsManager<T>> fn);
}
