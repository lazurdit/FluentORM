using System.Data.Common;

namespace LazurdIT.FluentOrm.Common;

public interface IInsertQuery<T> : ITableRelatedFluentQuery
    where T : IFluentModel, new()
{
    IFieldsSelectionManager<T> FieldsManager { get; }

    T? Execute(T record, bool returnNewRecord = false, DbConnection? connection = null);

    IInsertQuery<T> WithFields(Action<IFieldsSelectionManager<T>> fn);
}