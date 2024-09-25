using LazurdIT.FluentOrm.Common;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Pgsql;

public class PgsqlInsertQuery<T> : IInsertQuery<T> where T : IFluentModel, new()
{
    public PgsqlInsertQuery(NpgsqlConnection? connection = null)
    {
        TableName = PgsqlDtoMapper<T>.GetTableName();
        SqlConnection = connection;
    }

    public PgsqlInsertQuery<T> WithFields(Action<PgsqlFieldsSelectionManager<T>> fn)
    {
        fn(FieldsManager);
        return this;
    }

    public string ExpressionSymbol => "@";

    public PgsqlFieldsSelectionManager<T> FieldsManager { get; } = new();

    public string TableName { get; set; }

    public NpgsqlConnection? SqlConnection { get; set; }
    public DbConnection? Connection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    IFieldsSelectionManager<T> IInsertQuery<T>.FieldsManager => FieldsManager;

    public T? Execute(T record, bool returnNewRecord = false, NpgsqlConnection? sqlConnection = null)
    {
        // Use the provided connection or the default one
        var connection = sqlConnection ?? SqlConnection ?? throw new Exception("ConnectionNotPassed");

        // Ensure the connection is open
        var shouldCloseConnection = connection!.State == ConnectionState.Closed;
        if (shouldCloseConnection)
        {
            connection.Open();
        }
        string parameterName = "P1_";
        List<string> fieldNames = FieldsManager.FieldsList.GetFinalPropertyNames();
        string fieldsListString = string.Join(",", fieldNames);
        string fieldsParametersListString = string.Join(",", FieldsManager.FieldsList.GetFinalPropertyNames().Select(n => $"@{parameterName}{n}"));

        var insertQuery = $@"insert into {TableName} ({fieldsListString})
            values ({fieldsParametersListString}) {(returnNewRecord ? "RETURNING *" : "")};";

        NpgsqlCommand cmd = new(insertQuery, connection);

        cmd.Parameters.AddRange(FieldsManager.GetSqlParameters(record, parameterName).ToArray());

        if (returnNewRecord)
        {
            PgsqlDtoMapper<T> dtoMapper = new(FieldsManager.OriginalFieldsList);

            var dataReader = cmd.ExecuteReader();

            dataReader.Read();
            var result = dtoMapper.ToDtoModel(dataReader);
            return result;
        }
        else
        {
            cmd.ExecuteNonQuery();
            return default;
        }
    }

    T? IInsertQuery<T>.Execute(T record, bool returnNewRecord, DbConnection? sqlConnection) => Execute(record, returnNewRecord, sqlConnection as NpgsqlConnection);

    IInsertQuery<T> IInsertQuery<T>.WithFields(Action<IFieldsSelectionManager<T>> fn) => WithFields(fn);
}