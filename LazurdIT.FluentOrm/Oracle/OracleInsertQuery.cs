using System.Data;
using System.Data.Common;
using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;

namespace LazurdIT.FluentOrm.Oracle;

public class OracleInsertQuery<T> : IInsertQuery<T> where T : IFluentModel, new()
{
    public OracleInsertQuery<T> WithFields(Action<OracleFieldsSelectionManager<T>> fn)
    {
        fn(FieldsManager);
        return this;
    }

    public string ExpressionSymbol => ":";

    public OracleFieldsSelectionManager<T> FieldsManager { get; } = new();

    public string TableName { get; set; } = OracleDtoMapper<T>.GetTableName();

    public OracleConnection? SqlConnection { get; set; }
    public DbConnection? Connection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    IFieldsSelectionManager<T> IInsertQuery<T>.FieldsManager => FieldsManager;

    public OracleInsertQuery(OracleConnection? connection = null)
    {
        SqlConnection = connection;
    }

    public T? Execute(T record, bool returnNewRecord = false, OracleConnection? sqlConnection = null)
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
        string fieldsParametersListString = string.Join(",", FieldsManager.FieldsList.GetFinalPropertyNames().Select(n => $":{parameterName}{n}"));
        var fieldsList = FieldsManager.OriginalFieldsList.GetFinalPropertyNames();
        string returnFieldsParametersListString = $@"{string.Join(",", fieldsList)} INTO {string.Join(",", fieldsList.Select(n => $":new_{n}"))}";

        var insertQuery = $@"insert into {TableName} ({fieldsListString})
                values ({fieldsParametersListString}) {(returnNewRecord ? $" returning  {returnFieldsParametersListString}" : "")}";

        OracleCommand cmd = new(insertQuery, connection) { BindByName = true };

        cmd.Parameters.AddRange(FieldsManager.GetSqlParameters(record, parameterName).ToArray());

        if (returnNewRecord)
        {
            foreach (var field in FieldsManager.OriginalFieldsList)
            {
                var type = field.Value.Attribute.AttributeOracleDbType ?? OracleDbTypeConverter.GetDefaultDbType(field.Value.Property.PropertyType);

                cmd.Parameters.Add(new OracleParameter($"new_{field.Value.FinalPropertyName}", type, 300, ParameterDirection.ReturnValue));
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

    IInsertQuery<T> IInsertQuery<T>.WithFields(Action<IFieldsSelectionManager<T>> fn) => WithFields(fn);

    T? IInsertQuery<T>.Execute(T record, bool returnNewRecord, DbConnection? sqlConnection) => this.Execute(record, returnNewRecord, sqlConnection as OracleConnection);
}