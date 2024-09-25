using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.MsSql;

public class MsSqlUpdateQuery<T> : IUpdateQuery<T> where T : IFluentModel, new()
{
    public MsSqlConditionsManager<T> ConditionsManager { get; } = new();
    public MsSqlFluentUpdateCriteriaManager<T> UpdateManager { get; } = new();

    public string TableName { get; set; } = GetTableName();
    public string ExpressionSymbol => "@";

    private static string GetTableName()
    {
        var attribute = typeof(T).GetCustomAttribute<FluentTableAttribute>();
        string name = attribute?.Name ?? typeof(T).Name;
        return name;
    }

    public SqlConnection? SqlConnection { get; set; }
    DbConnection? IUpdateQuery<T>.Connection => SqlConnection;

    FluentUpdateCriteriaManager<T> IUpdateQuery<T>.UpdateManager => this.UpdateManager;

    IConditionsManager<T> IConditionQuery<T>.ConditionsManager => this.ConditionsManager;

    public MsSqlUpdateQuery(SqlConnection? sqlConnection = null)
    {
        SqlConnection = sqlConnection;
    }

    public int Execute(T record, SqlConnection? sqlConnection = null, bool ignoreEmptyConditions = false)
    => Execute(record, ConditionsManager, sqlConnection, ignoreEmptyConditions);

    public int Execute(T record, Action<MsSqlConditionsManager<T>> conditionsFn, SqlConnection? sqlConnection = null, bool ignoreEmptyConditions = false)
    {
        MsSqlConditionsManager<T> conditionsManager = new();
        conditionsFn(conditionsManager);
        return Execute(record, conditionsManager, sqlConnection, ignoreEmptyConditions);
    }

    public int Execute(T record, MsSqlConditionsManager<T> manager, SqlConnection? sqlConnection = null, bool ignoreEmptyConditions = false)
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

            StringBuilder updateQuery = new($@"update {TableName} set {expressions}");

            var parameters = new List<SqlParameter>();

            if (manager.WhereConditions.Count > 0)
            {
                int i = 0;
                updateQuery.Append(" WHERE " + string.Join(" AND ", manager.WhereConditions.Select(w => w.SetParameterName($"Wh_param_{++i}").GetExpression(ExpressionSymbol))));

                foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange((SqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!);
                }
            }
            else if (parameters.Count == 0 && !ignoreEmptyConditions)
            {
                var whereParameter = $"Wh_{parameterName}";
                MsSqlDtoMapper<T> dtoMapper = new();
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
            using var command = new SqlCommand(query, connection);
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

    public int Execute(SqlConnection? sqlConnection = null, bool ignoreEmptyConditions = false)
    => Execute(ConditionsManager, sqlConnection, ignoreEmptyConditions);

    public int Execute(Action<MsSqlConditionsManager<T>> conditionsFn, SqlConnection? sqlConnection = null, bool ignoreEmptyConditions = false)
    {
        MsSqlConditionsManager<T> conditionsManager = new();
        conditionsFn(conditionsManager);
        return Execute(conditionsManager, sqlConnection, ignoreEmptyConditions);
    }

    public int Execute(MsSqlConditionsManager<T> manager, SqlConnection? sqlConnection = null, bool ignoreEmptyConditions = false)
    {
        var connection = sqlConnection ?? SqlConnection ?? throw new Exception("ConnectionNotPassed");

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

            StringBuilder updateQuery = new($@"update {TableName} set {expressions}");

            var parameters = new List<SqlParameter>();

            if (manager.WhereConditions.Count > 0)
            {
                int i = 0;
                updateQuery.Append(" WHERE " + string.Join(" AND ", manager.WhereConditions.Select(w => w.SetParameterName($"Wh_param_{++i}").GetExpression(ExpressionSymbol))));

                foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange((SqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!);
                }
            }
            else if (parameters.Count == 0 && !ignoreEmptyConditions)
                throw new Exception("NoConditionsPassedAndCannotDeterminePK");

            //Add initial parameters
            var initialParameters = UpdateManager.GetSqlParameters(default, parameterName)?.ToList();

            if ((initialParameters?.Count ?? 0) > 0)
                parameters.AddRange(initialParameters!);

            string query = updateQuery.ToString();
            using var command = new SqlCommand(query, connection);
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

    public MsSqlUpdateQuery<T> Where(Action<MsSqlConditionsManager<T>> fn)
    {
        fn(ConditionsManager);
        return this;
    }

    public MsSqlUpdateQuery<T> WithFields(Action<FluentUpdateCriteriaManager<T>> fn)
    {
        fn(UpdateManager);
        return this;
    }

    int IUpdateQuery<T>.Execute(T record, DbConnection? sqlConnection, bool ignoreEmptyConditions) => Execute(record, (SqlConnection?)sqlConnection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(T record, Action<IConditionsManager<T>> conditionsFn, DbConnection? sqlConnection, bool ignoreEmptyConditions) => Execute(record, conditionsFn, (SqlConnection?)sqlConnection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(T record, IConditionsManager<T> manager, DbConnection? sqlConnection, bool ignoreEmptyConditions) => Execute(record, (MsSqlConditionsManager<T>)manager, (SqlConnection?)sqlConnection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(DbConnection? sqlConnection, bool ignoreEmptyConditions) => Execute((SqlConnection?)sqlConnection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(Action<IConditionsManager<T>> conditionsFn, DbConnection? sqlConnection, bool ignoreEmptyConditions) => Execute(conditionsFn, (SqlConnection?)sqlConnection, ignoreEmptyConditions);

    int IUpdateQuery<T>.Execute(IConditionsManager<T> manager, DbConnection? sqlConnection, bool ignoreEmptyConditions) => Execute((MsSqlConditionsManager<T>)manager, (SqlConnection?)sqlConnection, ignoreEmptyConditions);

    IUpdateQuery<T> IUpdateQuery<T>.WithFields(Action<FluentUpdateCriteriaManager<T>> fn) => WithFields(fn);

    IUpdateQuery<T> IUpdateQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);
}