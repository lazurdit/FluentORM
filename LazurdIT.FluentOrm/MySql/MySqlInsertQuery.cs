using System.Data;
using System.Data.Common;
using System.Text;
using LazurdIT.FluentOrm.Common;
using MySqlConnector;

namespace LazurdIT.FluentOrm.MySql;

public class MySqlInsertQuery<T> : IInsertQuery<T>
    where T : IFluentModel, new()
{
    public MySqlInsertQuery<T> WithFields(Action<MySqlFieldsSelectionManager<T>> fn)
    {
        fn(FieldsManager);
        return this;
    }

    public MySqlFieldsSelectionManager<T> FieldsManager { get; } = new();

    public string TableName { get; set; } = MySqlDtoMapper<T>.GetTableName();

    public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

    public string TablePrefix { get; set; } = string.Empty;

    ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
    {
        this.TablePrefix = tablePrefix;
        return this;
    }

    public MySqlInsertQuery<T> WithPrefix(string tablePrefix)
    {
        this.TablePrefix = tablePrefix;
        return this;
    }

    IDbConnection? IFluentQuery.Connection
    {
        get => Connection;
        set => Connection = (MySqlConnection?)value;
    }

    IFluentQuery IFluentQuery.WithConnection(IDbConnection? connection)
    {
        this.Connection = (MySqlConnection?)connection;
        return this;
    }

    public MySqlInsertQuery<T> WithConnection(MySqlConnection? connection)
    {
        this.Connection = connection;
        return this;
    }

    public string ExpressionSymbol => "@";

    public MySqlConnection? Connection { get; set; }

    IFieldsSelectionManager<T> IInsertQuery<T>.FieldsManager => FieldsManager;

    public MySqlInsertQuery(MySqlConnection? connection = null)
    {
        Connection = connection;
    }

    public T? Execute(T record, bool returnNewRecord = false, MySqlConnection? connection = null)
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
                values ({fieldsParametersListString});";

        MySqlCommand cmd = new(insertQuery, dbConnection);

        cmd.Parameters.AddRange(FieldsManager.GetSqlParameters(record, parameterName).ToArray());

        cmd.ExecuteNonQuery();
        T newRecord = default!;
        if (returnNewRecord)
        {
            StringBuilder query = new($"SELECT  * FROM {TableNameWithPrefix}");
            var parameters = new List<MySqlParameter>();

            using var command = new MySqlCommand() { Connection = dbConnection };
            bool getResult = false;
            if (FieldsManager.IdentityFieldsList.Count > 0)
            {
                query.Append(
                    $" where {FieldsManager.IdentityFieldsList.FirstOrDefault().Value.FinalPropertyName} = {cmd.LastInsertedId}"
                );
                getResult = true;
            }
            else if (FieldsManager.PKFieldsList.Count > 0)
            {
                query.Append(
                    $" where {FieldsManager.PKFieldsList.FirstOrDefault().Value.FinalPropertyName} = {cmd.LastInsertedId}"
                );
                getResult = true;
            }
            else
            {
                if (FieldsManager.FieldsList.Count > 0)
                {
                    string parameterName2 = "P1_";

                    string params2 = string.Join(
                        " and ",
                        FieldsManager.FieldsList.Select(w =>
                            $"{w.Value.FinalPropertyName} = @{parameterName2}{w.Value.FinalPropertyName}"
                        )
                    );
                    query.Append($" WHERE {params2}");

                    cmd.Parameters.AddRange(
                        FieldsManager.GetSqlParameters(record, parameterName).ToArray()
                    );
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

    IInsertQuery<T> IInsertQuery<T>.WithFields(Action<IFieldsSelectionManager<T>> fn) =>
        WithFields(fn);

    T? IInsertQuery<T>.Execute(T record, bool returnNewRecord, DbConnection? connection) =>
        this.Execute(record, returnNewRecord, connection as MySqlConnection);
}