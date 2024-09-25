using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.MsSql;

public class MsSqlDeleteQuery<T> : IDeleteQuery<T> where T : IFluentModel, new()
{
    public MsSqlConditionsManager<T> ConditionsManager { get; } = new();
    public string TableName { get; set; } = GetTableName();

    private static string GetTableName()
    {
        var attribute = typeof(T).GetCustomAttribute<FluentTableAttribute>();
        string name = attribute?.Name ?? typeof(T).Name;
        return name;
    }

    public string ExpressionSymbol => "@";

    public SqlConnection? SqlConnection { get; set; }

    IDbConnection? IDeleteQuery<T>.Connection { get => SqlConnection; }

    IConditionsManager<T> IConditionQuery<T>.ConditionsManager => throw new NotImplementedException();

    public MsSqlDeleteQuery(SqlConnection? sqlConnection = null)
    {
        SqlConnection = sqlConnection;
    }

    public int Execute(SqlConnection? sqlConnection = null, bool deleteAll = false)
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
            var query = $"delete from {TableName}";
            var parameters = new List<SqlParameter>();

            if (ConditionsManager.WhereConditions.Count > 0)
            {
                int i = 0;
                query += " WHERE " + string.Join(" AND ", ConditionsManager.WhereConditions.Select(w => w.SetParameterName($"param_{++i}").GetExpression(ExpressionSymbol)));

                foreach (var condition in ConditionsManager.WhereConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange((SqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!);
                }
            }

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

    public MsSqlDeleteQuery<T> Where(Action<MsSqlConditionsManager<T>> fn)
    {
        fn(ConditionsManager);
        return this;
    }

    int IDeleteQuery<T>.Execute(DbConnection? sqlConnection, bool deleteAll) => Execute((SqlConnection?)sqlConnection, deleteAll);

    IDeleteQuery<T> IDeleteQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);
}