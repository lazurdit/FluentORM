using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using LazurdIT.FluentOrm.Common;
using MySqlConnector;

namespace LazurdIT.FluentOrm.MySql;

public class MySqlUpdateQuery<T> : IUpdateQuery<T>
    where T : IFluentModel, new()
{
    public MySqlConditionsManager<T> ConditionsManager { get; } = new();
    public MySqlFluentUpdateCriteriaManager<T> UpdateManager { get; } = new();

    public string TableName { get; set; } = MySqlDtoMapper<T>.GetTableName();

    public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

    public string TablePrefix { get; set; } = string.Empty;

    ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
    {
        this.TablePrefix = tablePrefix;
        return this;
    }

    public MySqlUpdateQuery<T> WithPrefix(string tablePrefix)
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

    public MySqlUpdateQuery<T> WithConnection(MySqlConnection? connection)
    {
        this.Connection = connection;
        return this;
    }

    public string ExpressionSymbol => "@";

    public MySqlConnection? Connection { get; set; }

    FluentUpdateCriteriaManager<T> IUpdateQuery<T>.UpdateManager => this.UpdateManager;

    IConditionsManager<T> IConditionQuery<T>.ConditionsManager => this.ConditionsManager;

    public MySqlUpdateQuery(MySqlConnection? connection = null)
    {
        Connection = connection;
    }

    public int Execute(
        T record,
        MySqlConnection? connection = null,
        bool ignoreEmptyConditions = false
    ) => Execute(record, ConditionsManager, connection, ignoreEmptyConditions);

    public int Execute(
        T record,
        Action<MySqlConditionsManager<T>> conditionsFn,
        MySqlConnection? connection = null,
        bool ignoreEmptyConditions = false
    )
    {
        MySqlConditionsManager<T> conditionsManager = new();
        conditionsFn(conditionsManager);
        return Execute(record, conditionsManager, connection, ignoreEmptyConditions);
    }

    public int Execute(
        T record,
        MySqlConditionsManager<T> manager,
        MySqlConnection? connection = null,
        bool ignoreEmptyConditions = false
    )
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

        try
        {
            if (UpdateManager.Criterias.Count == 0)
                throw new Exception("NoFieldsToUpdate");

            string expressions = string.Join(
                ",",
                UpdateManager.GetFinalExpressions(parameterName, "@")
            );

            StringBuilder updateQuery = new($@"update {TableNameWithPrefix.ToLower()} set {expressions}");

            var parameters = new List<MySqlParameter>();

            if (manager.WhereConditions.Count > 0)
            {
                int i = 0;
                updateQuery.Append(
                    " WHERE "
                        + string.Join(
                            " AND ",
                            manager.WhereConditions.Select(w =>
                                w.SetParameterName($"Wh_param_{++i}")
                                    .GetExpression(ExpressionSymbol)
                            )
                        )
                );

                foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange(
                        (MySqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
                    );
                }
            }
            else if (parameters.Count == 0 && !ignoreEmptyConditions)
            {
                var whereParameter = $"Wh_{parameterName}";
                MySqlDtoMapper<T> dtoMapper = new();
                parameters.AddRange(
                    dtoMapper.GetPrimaryKeySqlParameters(record, whereParameter).ToList()
                );
                if (parameters.Count == 0)
                    throw new Exception("NoConditionsPassedAndCannotDeterminePK");
                updateQuery = updateQuery.Append(
                    $" where {dtoMapper.GetPrimaryKeySqlWhereString(whereParameter)}"
                );
            }

            //Add initial parameters
            var initialParameters = UpdateManager.GetSqlParameters(record, parameterName)?.ToList();

            if ((initialParameters?.Count ?? 0) > 0)
                parameters.AddRange(initialParameters!);

            string query = updateQuery.ToString();
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

    public int Execute(MySqlConnection? connection = null, bool ignoreEmptyConditions = false) =>
        Execute(ConditionsManager, connection, ignoreEmptyConditions);

    public int Execute(
        Action<MySqlConditionsManager<T>> conditionsFn,
        MySqlConnection? connection = null,
        bool ignoreEmptyConditions = false
    )
    {
        MySqlConditionsManager<T> conditionsManager = new();
        conditionsFn(conditionsManager);
        return Execute(conditionsManager, connection, ignoreEmptyConditions);
    }

    public int Execute(
        MySqlConditionsManager<T> manager,
        MySqlConnection? connection = null,
        bool ignoreEmptyConditions = false
    )
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

        try
        {
            if (UpdateManager.Criterias.Count == 0)
                throw new Exception("NoFieldsToUpdate");

            string expressions = string.Join(
                ",",
                UpdateManager.GetFinalExpressions(parameterName, "@")
            );

            StringBuilder updateQuery = new($@"update {TableNameWithPrefix.ToLower()} set {expressions}");

            var parameters = new List<MySqlParameter>();

            if (manager.WhereConditions.Count > 0)
            {
                int i = 0;
                updateQuery.Append(
                    " WHERE "
                        + string.Join(
                            " AND ",
                            manager.WhereConditions.Select(w =>
                                w.SetParameterName($"Wh_param_{++i}")
                                    .GetExpression(ExpressionSymbol)
                            )
                        )
                );

                foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange(
                        (MySqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
                    );
                }
            }
            else if (parameters.Count == 0 && !ignoreEmptyConditions)
                throw new Exception("NoConditionsPassedAndCannotDeterminePK");

            //Add initial parameters
            var initialParameters = UpdateManager
                .GetSqlParameters(default, parameterName)
                ?.ToList();

            if ((initialParameters?.Count ?? 0) > 0)
                parameters.AddRange(initialParameters!);

            string query = updateQuery.ToString();
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

    public MySqlUpdateQuery<T> Where(Action<MySqlConditionsManager<T>> fn)
    {
        fn(ConditionsManager);
        return this;
    }

    public MySqlUpdateQuery<T> WithFields(Action<FluentUpdateCriteriaManager<T>> fn)
    {
        fn(UpdateManager);
        return this;
    }

    int IUpdateQuery<T>.Execute(T record, DbConnection? connection, bool ignoreEmptyConditions) =>
        Execute(record, (MySqlConnection?)connection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(
        T record,
        Action<IConditionsManager<T>> conditionsFn,
        DbConnection? connection,
        bool ignoreEmptyConditions
    ) => Execute(record, conditionsFn, (MySqlConnection?)connection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(
        T record,
        IConditionsManager<T> manager,
        DbConnection? connection,
        bool ignoreEmptyConditions
    ) =>
        Execute(
            record,
            (MySqlConditionsManager<T>)manager,
            (MySqlConnection?)connection,
            ignoreEmptyConditions
        );

    int IUpdateQuery<T>.Execute(DbConnection? connection, bool ignoreEmptyConditions) =>
        Execute((MySqlConnection?)connection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(
        Action<IConditionsManager<T>> conditionsFn,
        DbConnection? connection,
        bool ignoreEmptyConditions
    ) => Execute(conditionsFn, (MySqlConnection?)connection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(
        IConditionsManager<T> manager,
        DbConnection? connection,
        bool ignoreEmptyConditions
    ) =>
        Execute(
            (MySqlConditionsManager<T>)manager,
            (MySqlConnection?)connection,
            ignoreEmptyConditions
        );

    IUpdateQuery<T> IUpdateQuery<T>.WithFields(Action<FluentUpdateCriteriaManager<T>> fn) =>
        WithFields(fn);

    IUpdateQuery<T> IUpdateQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);
}