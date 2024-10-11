using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.MsSql;

public class MsSqlInsertQuery<T> : IInsertQuery<T>
    where T : IFluentModel, new()
{
    public MsSqlInsertQuery<T> WithFields(Action<MsSqlFieldsSelectionManager<T>> fn)
    {
        fn(FieldsManager);
        return this;
    }

    public string TableName { get; set; } = MsSqlDtoMapper<T>.GetTableName();

    public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

    public string TablePrefix { get; set; } = string.Empty;

    ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
    {
        this.TablePrefix = tablePrefix;
        return this;
    }

    public MsSqlInsertQuery<T> WithPrefix(string tablePrefix)
    {
        this.TablePrefix = tablePrefix;
        return this;
    }

    IDbConnection? IFluentQuery.Connection
    {
        get => Connection;
        set => Connection = (SqlConnection?)value;
    }

    IFluentQuery IFluentQuery.WithConnection(IDbConnection? connection)
    {
        this.Connection = (SqlConnection?)connection;
        return this;
    }

    public MsSqlInsertQuery<T> WithConnection(SqlConnection? connection)
    {
        this.Connection = connection;
        return this;
    }

    public string ExpressionSymbol => "@";

    public MsSqlFieldsSelectionManager<T> FieldsManager { get; } = new();

    public SqlConnection? Connection { get; set; }

    IFieldsSelectionManager<T> IInsertQuery<T>.FieldsManager => FieldsManager;

    public MsSqlInsertQuery(SqlConnection? connection = null)
    {
        Connection = connection;
    }

    public T? Execute(T record, bool returnNewRecord = false, SqlConnection? connection = null)
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
            {(returnNewRecord ? "Output inserted.*" : "")}
            values ({fieldsParametersListString});";

        using var cmd = new SqlCommand (insertQuery, dbConnection);

        cmd.Parameters.AddRange(FieldsManager.GetSqlParameters(record, parameterName).ToArray());

        if (returnNewRecord)
        {
            using var dataReader = cmd.ExecuteReader();

            MsSqlDtoMapper<T> dtoMapper = new(FieldsManager.OriginalFieldsList);

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

    public int? InsertBulk(
        T[] records,
        int chunkSize = int.MaxValue,
        SqlConnection? connection = null
    )
    {
        // Validate input
        if (records == null || records.Length == 0)
            return null;

        int totalInserted = 0;

        // Use the provided connection or the default one
        var dbConnection = connection ?? Connection ?? throw new Exception("ConnectionNotPassed");

        // Ensure the connection is open
        var shouldCloseConnection = dbConnection!.State == ConnectionState.Closed;
        if (shouldCloseConnection)
        {
            dbConnection.Open();
        }

        try
        {
            for (int i = 0; i < records.Length; i += chunkSize)
            {
                var chunk = records.Skip(i).Take(chunkSize).ToArray();
                var dataTable = FieldsManager.ToDataTable(chunk);

                using var bulkCopy = new SqlBulkCopy(dbConnection);
                bulkCopy.DestinationTableName = TableNameWithPrefix;
                dataTable
                    .Columns.Cast<DataColumn>()
                    .ToList()
                    .ForEach(x =>
                        bulkCopy.ColumnMappings.Add(
                            new SqlBulkCopyColumnMapping(x.ColumnName, x.ColumnName)
                        )
                    );

                bulkCopy.WriteToServer(dataTable);
                totalInserted += chunk.Length;
            }
        }
        finally
        {
            if (shouldCloseConnection && dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
            }
        }

        return totalInserted;
    }

    IInsertQuery<T> IInsertQuery<T>.WithFields(Action<IFieldsSelectionManager<T>> fn) =>
        WithFields(fn);

    T? IInsertQuery<T>.Execute(T record, bool returnNewRecord, DbConnection? connection) =>
        this.Execute(record, returnNewRecord, connection as SqlConnection);
}