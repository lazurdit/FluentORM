using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.MsSql;

public class MsSqlInsertQuery<T> : IInsertQuery<T> where T : IFluentModel, new()
{
    public MsSqlInsertQuery<T> WithFields(Action<MsSqlFieldsSelectionManager<T>> fn)
    {
        fn(FieldsManager);
        return this;
    }

    public string ExpressionSymbol => "@";

    public MsSqlFieldsSelectionManager<T> FieldsManager { get; } = new();

    public string TableName { get; set; } = MsSqlDtoMapper<T>.GetTableName();

    public SqlConnection? SqlConnection { get; set; }
    public DbConnection? Connection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    IFieldsSelectionManager<T> IInsertQuery<T>.FieldsManager => FieldsManager;

    public MsSqlInsertQuery(SqlConnection? connection = null)
    {
        SqlConnection = connection;
    }

    public T? Execute(T record, bool returnNewRecord = false, SqlConnection? sqlConnection = null)
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
            {(returnNewRecord ? "Output inserted.*" : "")}
            values ({fieldsParametersListString});";

        SqlCommand cmd = new(insertQuery, connection);

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

    public int? InsertBulk(T[] records, int chunkSize = int.MaxValue, SqlConnection? sqlConnection = null)
    {
        // Validate input
        if (records == null || records.Length == 0)
            return null;

        int totalInserted = 0;

        // Use the provided connection or the default one
        var connection = sqlConnection ?? SqlConnection ?? throw new Exception("ConnectionNotPassed");

        // Ensure the connection is open
        var shouldCloseConnection = connection!.State == ConnectionState.Closed;
        if (shouldCloseConnection)
        {
            connection.Open();
        }

        try
        {
            for (int i = 0; i < records.Length; i += chunkSize)
            {
                var chunk = records.Skip(i).Take(chunkSize).ToArray();
                var dataTable = FieldsManager.ToDataTable(chunk);

                using var bulkCopy = new SqlBulkCopy(connection);
                bulkCopy.DestinationTableName = TableName;
                dataTable.Columns.Cast<DataColumn>().ToList().ForEach(x =>
                    bulkCopy.ColumnMappings.Add(new SqlBulkCopyColumnMapping(x.ColumnName, x.ColumnName)));

                bulkCopy.WriteToServer(dataTable);
                totalInserted += chunk.Length;
            }
        }
        finally
        {
            if (shouldCloseConnection && connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        return totalInserted;
    }

    IInsertQuery<T> IInsertQuery<T>.WithFields(Action<IFieldsSelectionManager<T>> fn) => WithFields(fn);

    T? IInsertQuery<T>.Execute(T record, bool returnNewRecord, DbConnection? sqlConnection) => this.Execute(record, returnNewRecord, sqlConnection as SqlConnection);
}