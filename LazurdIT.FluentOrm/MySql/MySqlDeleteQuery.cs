using System.Data;
using System.Data.Common;
using System.Reflection;
using LazurdIT.FluentOrm.Common;
using MySqlConnector;

namespace LazurdIT.FluentOrm.MySql;

public class MySqlDeleteQuery<T> : IDeleteQuery<T>
    where T : IFluentModel, new()
{
    public MySqlConditionsManager<T> ConditionsManager { get; } = new();

    public string TableName { get; set; } = MySqlDtoMapper<T>.GetTableName();

    public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

    public string TablePrefix { get; set; } = string.Empty;

    ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
    {
        this.TablePrefix = tablePrefix;
        return this;
    }

    public MySqlDeleteQuery<T> WithPrefix(string tablePrefix)
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

    public MySqlDeleteQuery<T> WithConnection(MySqlConnection? connection)
    {
        this.Connection = connection;
        return this;
    }

    public string ExpressionSymbol => "@";
    public MySqlConnection? Connection { get; set; }

    IConditionsManager<T> IConditionQuery<T>.ConditionsManager => this.ConditionsManager;

    public MySqlDeleteQuery(MySqlConnection? connection = null)
    {
        Connection = connection;
    }

    public int Execute(MySqlConnection? connection = null, bool deleteAll = false)
    {
        // Use the provided connection or the default one
        var dbConnection = connection ?? Connection ?? throw new Exception("ConnectionNotPassed");
        if (!deleteAll && ConditionsManager.WhereConditions.Count == 0)
            throw new Exception("DeleteAllMustBeTrueIfNoWhereConditionPassed");

        // Ensure the connection is open
        var shouldCloseConnection = dbConnection!.State == ConnectionState.Closed;
        if (shouldCloseConnection)
        {
            dbConnection.Open();
        }

        try
        {
            var query = $"delete from {TableNameWithPrefix.ToLower()}";
            var parameters = new List<MySqlParameter>();

            if (ConditionsManager.WhereConditions.Count > 0)
            {
                int i = 0;
                query +=
                    " WHERE "
                    + string.Join(
                        " AND ",
                        ConditionsManager.WhereConditions.Select(w =>
                            w.SetParameterName($"param_{++i}").GetExpression(ExpressionSymbol)
                        )
                    );

                foreach (
                    var condition in ConditionsManager.WhereConditions.Where(w => w.HasParameters)
                )
                {
                    parameters.AddRange(
                        (MySqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
                    );
                }
            }

            using var command = new MySqlCommand(query, dbConnection);
            if (parameters.Count > 0)
                command.Parameters.AddRange(parameters.ToArray());

            int count = command.ExecuteNonQuery();
            return count;
        }
        finally
        {
            if (shouldCloseConnection)
                dbConnection.Close();
        }
    }

    public MySqlDeleteQuery<T> Where(Action<MySqlConditionsManager<T>> fn)
    {
        fn(ConditionsManager);
        return this;
    }

    int IDeleteQuery<T>.Execute(DbConnection? connection, bool deleteAll) =>
        Execute((MySqlConnection?)connection, deleteAll);

    IDeleteQuery<T> IDeleteQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);
}
