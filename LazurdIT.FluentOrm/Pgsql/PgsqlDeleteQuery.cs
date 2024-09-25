using LazurdIT.FluentOrm.Common;
using Npgsql;
using System.Data;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Pgsql;

public class PgsqlDeleteQuery<T> : IConditionQuery<T>, IDeleteQuery<T> where T : IFluentModel, new()
{
    public PgsqlConditionsManager<T> ConditionsManager { get; } = new();
    public string TableName { get; set; } = PgsqlDtoMapper<T>.GetTableName();

    public NpgsqlConnection? SqlConnection { get; set; }

    public string ExpressionSymbol => "@";
    IDbConnection? IDeleteQuery<T>.Connection { get => SqlConnection; }

    public int Execute(NpgsqlConnection? sqlConnection = null, bool deleteAll = false)
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
            var parameters = new List<NpgsqlParameter>();

            if (ConditionsManager.WhereConditions.Count > 0)
            {
                int i = 0;
                query += " WHERE " + string.Join(" AND ", ConditionsManager.WhereConditions.Select(w => w.SetParameterName($"param_{++i}").GetExpression(ExpressionSymbol)));

                foreach (var condition in ConditionsManager.WhereConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange((NpgsqlParameter[])condition.GetDbParameters(ExpressionSymbol)!);
                }
            }

            using var command = new NpgsqlCommand(query, connection);
            if (parameters.Count > 0)
                foreach (var parameter in parameters)
                    command.Parameters.Add(parameter);

            int count = command.ExecuteNonQuery();
            return count;
        }
        finally
        {
            if (shouldCloseConnection)
                connection.Close();
        }
    }

    public PgsqlDeleteQuery<T> Where(Action<PgsqlConditionsManager<T>> fn)
    {
        fn(ConditionsManager);
        return this;
    }

    int IDeleteQuery<T>.Execute(DbConnection? sqlConnection, bool deleteAll) => Execute((NpgsqlConnection?)sqlConnection, deleteAll);

    IDeleteQuery<T> IDeleteQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);

    IConditionsManager<T> IConditionQuery<T>.ConditionsManager => ConditionsManager;

    public PgsqlDeleteQuery(NpgsqlConnection? sqlConnection = null)
    {
        SqlConnection = sqlConnection;
    }
}