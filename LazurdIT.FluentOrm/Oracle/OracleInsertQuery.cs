using System.Data;
using System.Data.Common;
using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;

namespace LazurdIT.FluentOrm.Oracle;

public class OracleInsertQuery<T> : IInsertQuery<T>
    where T : IFluentModel, new()
{
    public OracleInsertQuery<T> WithFields(Action<OracleFieldsSelectionManager<T>> fn)
    {
        fn(FieldsManager);
        return this;
    }

    public string TableName { get; set; } = OracleDtoMapper<T>.GetTableName();

    public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

    public string TablePrefix { get; set; } = string.Empty;

    ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
    {
        this.TablePrefix = tablePrefix;
        return this;
    }

    public OracleInsertQuery<T> WithPrefix(string tablePrefix)
    {
        this.TablePrefix = tablePrefix;
        return this;
    }

    IDbConnection? IFluentQuery.Connection
    {
        get => Connection;
        set => Connection = (OracleConnection?)value;
    }

    IFluentQuery IFluentQuery.WithConnection(IDbConnection? connection)
    {
        this.Connection = (OracleConnection?)connection;
        return this;
    }

    public OracleInsertQuery<T> WithConnection(OracleConnection? connection)
    {
        this.Connection = connection;
        return this;
    }

    public string ExpressionSymbol => ":";

    public OracleFieldsSelectionManager<T> FieldsManager { get; } = new();

    public OracleConnection? Connection { get; set; }

    IFieldsSelectionManager<T> IInsertQuery<T>.FieldsManager => FieldsManager;

    public OracleInsertQuery(OracleConnection? connection = null)
    {
        Connection = connection;
    }

    public T? Execute(T record, bool returnNewRecord = false, OracleConnection? connection = null)
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
            FieldsManager.FieldsList.GetFinalPropertyNames().Select(n => $":{parameterName}{n}")
        );
        var fieldsList = FieldsManager.OriginalFieldsList.GetFinalPropertyNames();
        string returnFieldsParametersListString =
            $@"{string.Join(",", fieldsList)} INTO {string.Join(",", fieldsList.Select(n => $":new_{n}"))}";

        var insertQuery =
            $@"insert into {TableNameWithPrefix} ({fieldsListString})
                values ({fieldsParametersListString}) {(returnNewRecord ? $" returning  {returnFieldsParametersListString}" : "")}";

        using var cmd = new OracleCommand(insertQuery, dbConnection) { BindByName = true };

        cmd.Parameters.AddRange(FieldsManager.GetSqlParameters(record, parameterName).ToArray());

        if (returnNewRecord)
        {
            foreach (var field in FieldsManager.OriginalFieldsList)
            {
                var type =
                    field.Value.Attribute.AttributeOracleDbType
                    ?? OracleDbTypeConverter.GetDefaultDbType(field.Value.Property.PropertyType);

                cmd.Parameters.Add(
                    new OracleParameter(
                        $"new_{field.Value.FinalPropertyName}",
                        type,
                        300,
                        ParameterDirection.ReturnValue
                    )
                );
            }
        }
        var c = cmd.ExecuteScalar();

        T? newRecord = default!;
        if (returnNewRecord)
        {
            OracleDtoMapper<T> mapper = new();
            newRecord = mapper.ToDtoModel(cmd, "new_");
        }

        return newRecord;
    }

    IInsertQuery<T> IInsertQuery<T>.WithFields(Action<IFieldsSelectionManager<T>> fn) =>
        WithFields(fn);

    T? IInsertQuery<T>.Execute(T record, bool returnNewRecord, DbConnection? connection) =>
        this.Execute(record, returnNewRecord, connection as OracleConnection);
}