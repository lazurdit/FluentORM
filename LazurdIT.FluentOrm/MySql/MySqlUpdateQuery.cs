using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using LazurdIT.FluentOrm.Common;
using MySqlConnector;

namespace LazurdIT.FluentOrm.MySql;

public class MySqlUpdateQuery<T> : IUpdateQuery<T> where T : IFluentModel, new()
{
    public MySqlConditionsManager<T> ConditionsManager { get; } = new();
    public MySqlFluentUpdateCriteriaManager<T> UpdateManager { get; } = new();

    public string TableName { get; set; } = GetTableName();
    public string ExpressionSymbol => "@";

    private static string GetTableName()
    {
        var attribute = typeof(T).GetCustomAttribute<FluentTableAttribute>();
        string name = attribute?.Name ?? typeof(T).Name;
        return name;
    }

    public MySqlConnection? SqlConnection { get; set; }
    DbConnection? IUpdateQuery<T>.Connection => SqlConnection;

    FluentUpdateCriteriaManager<T> IUpdateQuery<T>.UpdateManager => this.UpdateManager;

    IConditionsManager<T> IConditionQuery<T>.ConditionsManager => this.ConditionsManager;

    public MySqlUpdateQuery(MySqlConnection? sqlConnection = null)
    {
        SqlConnection = sqlConnection;
    }

    public int Execute(T record, MySqlConnection? sqlConnection = null, bool ignoreEmptyConditions = false)
    => Execute(record, ConditionsManager, sqlConnection, ignoreEmptyConditions);

    public int Execute(T record, Action<MySqlConditionsManager<T>> conditionsFn, MySqlConnection? sqlConnection = null, bool ignoreEmptyConditions = false)
    {
        MySqlConditionsManager<T> conditionsManager = new();
        conditionsFn(conditionsManager);
        return Execute(record, conditionsManager, sqlConnection, ignoreEmptyConditions);
    }

    public int Execute(T record, MySqlConditionsManager<T> manager, MySqlConnection? sqlConnection = null, bool ignoreEmptyConditions = false)
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

        try
        {
            if (UpdateManager.Criterias.Count == 0)
                throw new Exception("NoFieldsToUpdate");

            string expressions = string.Join(",", UpdateManager.GetFinalExpressions(parameterName, "@"));

            StringBuilder updateQuery = new($@"update {TableName.ToLower()} set {expressions}");

            var parameters = new List<MySqlParameter>();

            if (manager.WhereConditions.Count > 0)
            {
                int i = 0;
                updateQuery.Append(" WHERE " + string.Join(" AND ", manager.WhereConditions.Select(w => w.SetParameterName($"Wh_param_{++i}").GetExpression(ExpressionSymbol))));

                foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange((MySqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!);
                }
            }
            else if (parameters.Count == 0 && !ignoreEmptyConditions)
            {
                var whereParameter = $"Wh_{parameterName}";
                MySqlDtoMapper<T> dtoMapper = new();
                parameters.AddRange(dtoMapper.GetPrimaryKeySqlParameters(record, whereParameter).ToList());
                if (parameters.Count == 0)
                    throw new Exception("NoConditionsPassedAndCannotDeterminePK");
                updateQuery = updateQuery.Append($" where {dtoMapper.GetPrimaryKeySqlWhereString(whereParameter)}");
            }

            //Add initial parameters
            var initialParameters = UpdateManager.GetSqlParameters(record, parameterName)?.ToList();

            if ((initialParameters?.Count ?? 0) > 0)
                parameters.AddRange(initialParameters!);

            string query = updateQuery.ToString();
            using var command = new MySqlCommand(query, connection);
            if (parameters.Count > 0)
                command.Parameters.AddRange(parameters.ToArray());
            int count = command.ExecuteNonQuery();
            return count;
        }
        finally
        {
            if (shouldCloseConnection)
                connection.Close();
        }
    }

    public int Execute(MySqlConnection? sqlConnection = null, bool ignoreEmptyConditions = false)
    => Execute(ConditionsManager, sqlConnection, ignoreEmptyConditions);

    public int Execute(Action<MySqlConditionsManager<T>> conditionsFn, MySqlConnection? sqlConnection = null, bool ignoreEmptyConditions = false)
    {
        MySqlConditionsManager<T> conditionsManager = new();
        conditionsFn(conditionsManager);
        return Execute(conditionsManager, sqlConnection, ignoreEmptyConditions);
    }

    public int Execute(MySqlConditionsManager<T> manager, MySqlConnection? sqlConnection = null, bool ignoreEmptyConditions = false)
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

        try
        {
            if (UpdateManager.Criterias.Count == 0)
                throw new Exception("NoFieldsToUpdate");

            string expressions = string.Join(",", UpdateManager.GetFinalExpressions(parameterName, "@"));

            StringBuilder updateQuery = new($@"update {TableName.ToLower()} set {expressions}");

            var parameters = new List<MySqlParameter>();

            if (manager.WhereConditions.Count > 0)
            {
                int i = 0;
                updateQuery.Append(" WHERE " + string.Join(" AND ", manager.WhereConditions.Select(w => w.SetParameterName($"Wh_param_{++i}").GetExpression(ExpressionSymbol))));

                foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange((MySqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!);
                }
            }
            else if (parameters.Count == 0 && !ignoreEmptyConditions)
                throw new Exception("NoConditionsPassedAndCannotDeterminePK");

            //Add initial parameters
            var initialParameters = UpdateManager.GetSqlParameters(default, parameterName)?.ToList();

            if ((initialParameters?.Count ?? 0) > 0)
                parameters.AddRange(initialParameters!);

            string query = updateQuery.ToString();
            using var command = new MySqlCommand(query, connection);
            if (parameters.Count > 0)
                command.Parameters.AddRange(parameters.ToArray());
            int count = command.ExecuteNonQuery();
            return count;
        }
        finally
        {
            if (shouldCloseConnection)
                connection.Close();
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

    int IUpdateQuery<T>.Execute(T record, DbConnection? sqlConnection, bool ignoreEmptyConditions) => Execute(record, (MySqlConnection?)sqlConnection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(T record, Action<IConditionsManager<T>> conditionsFn, DbConnection? sqlConnection, bool ignoreEmptyConditions) => Execute(record, conditionsFn, (MySqlConnection?)sqlConnection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(T record, IConditionsManager<T> manager, DbConnection? sqlConnection, bool ignoreEmptyConditions) => Execute(record, (MySqlConditionsManager<T>)manager, (MySqlConnection?)sqlConnection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(DbConnection? sqlConnection, bool ignoreEmptyConditions) => Execute((MySqlConnection?)sqlConnection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(Action<IConditionsManager<T>> conditionsFn, DbConnection? sqlConnection, bool ignoreEmptyConditions) => Execute(conditionsFn, (MySqlConnection?)sqlConnection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(IConditionsManager<T> manager, DbConnection? sqlConnection, bool ignoreEmptyConditions) => Execute((MySqlConditionsManager<T>)manager, (MySqlConnection?)sqlConnection, ignoreEmptyConditions);

    IUpdateQuery<T> IUpdateQuery<T>.WithFields(Action<FluentUpdateCriteriaManager<T>> fn) => WithFields(fn);

    IUpdateQuery<T> IUpdateQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);
}