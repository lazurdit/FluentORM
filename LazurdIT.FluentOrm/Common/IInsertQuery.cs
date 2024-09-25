using System.Data.Common;

namespace LazurdIT.FluentOrm.Common;

public interface IInsertQuery<T> : IFluentQuery where T : IFluentModel, new()
{
    IFieldsSelectionManager<T> FieldsManager { get; }
    DbConnection? Connection { get; set; }
    string TableName { get; set; }

    T? Execute(T record, bool returnNewRecord = false, DbConnection? sqlConnection = null);

    IInsertQuery<T> WithFields(Action<IFieldsSelectionManager<T>> fn);
}