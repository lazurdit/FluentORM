using LazurdIT.FluentOrm.Common;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace LazurdIT.FluentOrm.Pgsql;

public class PgsqlSelectQuery<T> : ISelectQuery<T> where T : IFluentModel, new()
{
    public PgsqlConditionsManager<T> ConditionsManager { get; } = new();
    public OrderByManager<T> OrderByManager { get; } = new();
    public PgsqlFieldsSelectionManager<T> FieldsManager { get; } = new();

    public string ExpressionSymbol => "@";
    public string TableName { get; set; } = GetTableName();

    private static string GetTableName()
    {
        var attribute = typeof(T).GetCustomAttribute<FluentTableAttribute>();
        string name = attribute?.Name ?? typeof(T).Name;
        return name;
    }

    public NpgsqlConnection? SqlConnection { get; set; }
    DbConnection? ISelectQuery<T>.Connection => SqlConnection;

    IConditionsManager<T> IConditionQuery<T>.ConditionsManager => ConditionsManager;

    IFieldsSelectionManager<T> ISelectQuery<T>.FieldsManager => FieldsManager;

    public PgsqlSelectQuery(NpgsqlConnection? sqlConnection = null)
    {
        SqlConnection = sqlConnection;
    }

    public IEnumerable<T> Execute(NpgsqlConnection? sqlConnection = null, int pageNumber = 0, int recordsCount = 0)
    {
        // Use the provided connection or the default one
        var connection = sqlConnection ?? SqlConnection ?? throw new Exception("ConnectionNotPassed");

        // Ensure the connection is open
        var shouldCloseConnection = connection!.State == ConnectionState.Closed;
        if (shouldCloseConnection)
        {
            connection.Open();
        }
        string includeColumns = FieldsManager.FieldsList.Count > 0 ? string.Join(",", FieldsManager.FieldsList.GetFinalPropertyNames()) : "*";

        try
        {
            StringBuilder query = new($"SELECT {(recordsCount > 0 && pageNumber <= 0 ? $"TOP {recordsCount}" : "")} {includeColumns} FROM {TableName}");
            var parameters = new List<SqlParameter>();

            if (ConditionsManager.WhereConditions.Count > 0)
            {
                int i = 0;
                query.Append(" WHERE " + string.Join(" AND ", ConditionsManager.WhereConditions.Select(w => w.SetParameterName($"param_{++i}").GetExpression(ExpressionSymbol))));

                foreach (var condition in ConditionsManager.WhereConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange((SqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!);
                }
            }

            if (OrderByManager.OrderByColumns?.Count > 0)
                query.Append(" ORDER BY " + string.Join(", ", OrderByManager.OrderByColumns.Select(o => o.Expression)));

            if (pageNumber > 0 && recordsCount > 0)
                query.Append($" {(OrderByManager.OrderByColumns?.Count > 0 ? "" : "order by (select null)")} OFFSET {pageNumber * recordsCount} ROWS FETCH NEXT {recordsCount} ROWS ONLY");

            using var command = new NpgsqlCommand(query.ToString(), connection);
            if (parameters.Count > 0)
                command.Parameters.AddRange(parameters.ToArray());

            using var dataReader = command.ExecuteReader();

            PgsqlDtoMapper<T> dtoMapper = new(FieldsManager.FieldsList);

            while (dataReader.Read())
            {
                var student = dtoMapper.ToDtoModel(dataReader);
                yield return student;
            }
        }
        finally
        {
            if (shouldCloseConnection)
                connection.Close();
        }
    }

    public PgsqlSelectQuery<T> Where(Action<PgsqlConditionsManager<T>> fn) => Where(fn);

    public PgsqlSelectQuery<T> OrderBy(Action<OrderByManager<T>> fn) => OrderBy(fn);

    public PgsqlSelectQuery<T> Returns(Action<PgsqlFieldsSelectionManager<T>> fn) => Returns(fn);

    IEnumerable<T> ISelectQuery<T>.Execute(DbConnection? sqlConnection, int pageNumber, int recordsCount) => Execute((NpgsqlConnection?)sqlConnection, pageNumber, recordsCount);

    ISelectQuery<T> ISelectQuery<T>.OrderBy(Action<OrderByManager<T>> fn) => OrderBy(fn);

    ISelectQuery<T> ISelectQuery<T>.Returns(Action<IFieldsSelectionManager<T>> fn) => Returns(fn);

    ISelectQuery<T> ISelectQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);
}