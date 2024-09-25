using System.Data;
using System.Data.Common;
using System.Text;
using LazurdIT.FluentOrm.Common;
using MySqlConnector;

namespace LazurdIT.FluentOrm.MySql;

public class MySqlInsertQuery<T> : IInsertQuery<T> where T : IFluentModel, new()
{
    public MySqlInsertQuery<T> WithFields(Action<MySqlFieldsSelectionManager<T>> fn)
    {
        fn(FieldsManager);
        return this;
    }

    public MySqlFieldsSelectionManager<T> FieldsManager { get; } = new();

    public string TableName { get; set; } = MySqlDtoMapper<T>.GetTableName();
    public string ExpressionSymbol => "@";

    public MySqlConnection? SqlConnection { get; set; }
    public DbConnection? Connection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    IFieldsSelectionManager<T> IInsertQuery<T>.FieldsManager => FieldsManager;

    public MySqlInsertQuery(MySqlConnection? connection = null)
    {
        SqlConnection = connection;
    }

    public T? Execute(T record, bool returnNewRecord = false, MySqlConnection? sqlConnection = null)
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
                values ({fieldsParametersListString});";

        MySqlCommand cmd = new(insertQuery, connection);

        cmd.Parameters.AddRange(FieldsManager.GetSqlParameters(record, parameterName).ToArray());

        cmd.ExecuteNonQuery();
        T newRecord = default!;
        if (returnNewRecord)
        {
            StringBuilder query = new($"SELECT  * FROM {TableName}");
            var parameters = new List<MySqlParameter>();

            using var command = new MySqlCommand()
            {
                Connection = connection
            };
            bool getResult = false;
            if (FieldsManager.IdentityFieldsList.Count > 0)
            {
                query.Append($" where {FieldsManager.IdentityFieldsList.FirstOrDefault().Value.FinalPropertyName} = {cmd.LastInsertedId}");
                getResult = true;
            }
            else if (FieldsManager.PKFieldsList.Count > 0)
            {
                query.Append($" where {FieldsManager.PKFieldsList.FirstOrDefault().Value.FinalPropertyName} = {cmd.LastInsertedId}");
                getResult = true;
            }
            else
            {
                if (FieldsManager.FieldsList.Count > 0)
                {
                    string parameterName2 = "P1_";

                    string params2 = string.Join(" and ", FieldsManager.FieldsList.Select(w => $"{w.Value.FinalPropertyName} = @{parameterName2}{w.Value.FinalPropertyName}"));
                    query.Append($" WHERE {params2}");

                    cmd.Parameters.AddRange(FieldsManager.GetSqlParameters(record, parameterName).ToArray());
                    getResult = true;
                }
            }
            if (getResult)
            {
                command.CommandText = query.ToString();

                using var dataReader = command.ExecuteReader();

                MySqlDtoMapper<T> dtoMapper = new(FieldsManager.OriginalFieldsList);

                if (dataReader.Read())
                    newRecord = dtoMapper.ToDtoModel(dataReader);
            }
        }

        return newRecord;
    }

    IInsertQuery<T> IInsertQuery<T>.WithFields(Action<IFieldsSelectionManager<T>> fn) => WithFields(fn);

    T? IInsertQuery<T>.Execute(T record, bool returnNewRecord, DbConnection? sqlConnection) => this.Execute(record, returnNewRecord, sqlConnection as MySqlConnection);
}