using System.Data;
using System.Data.Common;
using System.Reflection;
using LazurdIT.FluentOrm.Common;
using MySqlConnector;

namespace LazurdIT.FluentOrm.MySql;

public class MySqlDeleteQuery<T> : IDeleteQuery<T> where T : IFluentModel, new()
{
    public MySqlConditionsManager<T> ConditionsManager { get; } = new();
    public string TableName { get; set; } = GetTableName();

    private static string GetTableName()
    {
        var attribute = typeof(T).GetCustomAttribute<FluentTableAttribute>();
        string name = attribute?.Name ?? typeof(T).Name;
        return name;
    }

    public string ExpressionSymbol => "@";
    public MySqlConnection? SqlConnection { get; set; }

    IDbConnection? IDeleteQuery<T>.Connection { get => SqlConnection; }

    IConditionsManager<T> IConditionQuery<T>.ConditionsManager => throw new NotImplementedException();

    public MySqlDeleteQuery(MySqlConnection? sqlConnection = null)
    {
        SqlConnection = sqlConnection;
    }

    public int Execute(MySqlConnection? sqlConnection = null, bool deleteAll = false)
    {
        // Use the provided connection or the default one
        var connection = sqlConnection ?? SqlConnection ?? throw new Exception("ConnectionNotPassed");
        if (!deleteAll && ConditionsManager.WhereConditions.Count == 0)
            throw new Exception("DeleteAllMustBeTrueIfNoWhereConditionPassed");

        // Ensure the connection is open
        var shouldCloseConnection = connection!.State == ConnectionState.Closed;
        if (shouldCloseConnection)
        {
            connection.Open();
        }

        try
        {
            var query = $"delete from {TableName.ToLower()}";
            var parameters = new List<MySqlParameter>();

            if (ConditionsManager.WhereConditions.Count > 0)
            {
                int i = 0;
                query += " WHERE " + string.Join(" AND ", ConditionsManager.WhereConditions.Select(w => w.SetParameterName($"param_{++i}").GetExpression(ExpressionSymbol)));

                foreach (var condition in ConditionsManager.WhereConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange((MySqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!);
                }
            }

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

    public MySqlDeleteQuery<T> Where(Action<MySqlConditionsManager<T>> fn)
    {
        fn(ConditionsManager);
        return this;
    }

    int IDeleteQuery<T>.Execute(DbConnection? sqlConnection, bool deleteAll) => Execute((MySqlConnection?)sqlConnection, deleteAll);

    IDeleteQuery<T> IDeleteQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);
}