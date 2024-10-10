using System.Data;
using System.Data.Common;
using LazurdIT.FluentOrm.Common;
using Npgsql;

namespace LazurdIT.FluentOrm.Pgsql;

public class PgsqlInsertQuery<T> : IInsertQuery<T>
    where T : IFluentModel, new()
{
    public PgsqlInsertQuery(NpgsqlConnection? connection = null)
    {
        Connection = connection;
    }

    public PgsqlInsertQuery<T> WithFields(Action<PgsqlFieldsSelectionManager<T>> fn)
    {
        fn(FieldsManager);
        return this;
    }

    public string TableName { get; set; } = PgsqlDtoMapper<T>.GetTableName();

    public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

    public string TablePrefix { get; set; } = string.Empty;

    ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
    {
        this.TablePrefix = tablePrefix;
        return this;
    }

    public PgsqlInsertQuery<T> WithPrefix(string tablePrefix)
    {
        this.TablePrefix = tablePrefix;
        return this;
    }

    IDbConnection? IFluentQuery.Connection
    {
        get => Connection;
        set => Connection = (NpgsqlConnection?)value;
    }

    IFluentQuery IFluentQuery.WithConnection(IDbConnection? connection)
    {
        this.Connection = (NpgsqlConnection?)connection;
        return this;
    }

    public PgsqlInsertQuery<T> WithConnection(NpgsqlConnection? connection)
    {
        this.Connection = connection;
        return this;
    }

    public string ExpressionSymbol => "@";

    public PgsqlFieldsSelectionManager<T> FieldsManager { get; } = new();

    public NpgsqlConnection? Connection { get; set; }

    IFieldsSelectionManager<T> IInsertQuery<T>.FieldsManager => FieldsManager;

    public T? Execute(T record, bool returnNewRecord = false, NpgsqlConnection? connection = null)
    {
        // Use the provided connection or the default one
        var dbConnection = connection ?? Connection ?? throw new Exception("ConnectionNotPassed");

        // Ensure the connection is open
        var shouldCloseConnection = dbConnection!.State == ConnectionState.Closed;
        if (shouldCloseConnection)
        {
            dbConnection.Open();
        }
        string parameterName = "P1_";
        List<string> fieldNames = FieldsManager.FieldsList.GetFinalPropertyNames();
        string fieldsListString = string.Join(",", fieldNames);
        string fieldsParametersListString = string.Join(
            ",",
            FieldsManager.FieldsList.GetFinalPropertyNames().Select(n => $"@{parameterName}{n}")
        );

        var insertQuery =
            $@"insert into {TableNameWithPrefix} ({fieldsListString})
            values ({fieldsParametersListString}) {(returnNewRecord ? "RETURNING *" : "")};";

        NpgsqlCommand cmd = new(insertQuery, dbConnection);

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

    T? IInsertQuery<T>.Execute(T record, bool returnNewRecord, DbConnection? connection) =>
        Execute(record, returnNewRecord, connection as NpgsqlConnection);

    IInsertQuery<T> IInsertQuery<T>.WithFields(Action<IFieldsSelectionManager<T>> fn) =>
        WithFields(fn);
}