using System.Data;
using System.Data.Common;
using System.Text;
using LazurdIT.FluentOrm.Common;
using MySqlConnector;

namespace LazurdIT.FluentOrm.MySql;

public class MySqlRawSelectQuery<T> : IRawSelectQuery<T> where T : IFluentModel, new()
{
    public MySqlConditionsManager<T> ConditionsManager { get; } = new();
    public OrderByManager<T> OrderByManager { get; } = new();
    public MySqlFieldsSelectionManager<T> FieldsManager { get; } = new();

    public string ExpressionSymbol => "@";
    public string SelectClause { get; set; } = string.Empty;

    public MySqlConnection? SqlConnection { get; set; }

    IConditionsManager<T> IConditionQuery<T>.ConditionsManager => ConditionsManager;

    DbConnection? IRawSelectQuery<T>.Connection => SqlConnection;

    IFieldsSelectionManager<T> IRawSelectQuery<T>.FieldsManager => FieldsManager;

    public MySqlRawSelectQuery(MySqlConnection? sqlConnection = null)
    {
        SqlConnection = sqlConnection;
    }

    public IEnumerable<T> Execute(MySqlConnection? sqlConnection = null, int pageNumber = 0, int recordsCount = 0)
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
            StringBuilder query = new(SelectClause);
            var parameters = new List<MySqlParameter>();

            if (ConditionsManager.WhereConditions.Count > 0)
            {
                int i = 0;
                query.Append($" WHERE {string.Join(" AND ", ConditionsManager.WhereConditions.Select(w => w.SetParameterName($"param_{++i}").GetExpression(ExpressionSymbol)))}");

                foreach (var condition in ConditionsManager.WhereConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange((MySqlParameter[])condition.GetDbParameters(ExpressionSymbol)!);
                }
            }

            if (OrderByManager.OrderByColumns?.Count > 0)
                query.Append(" ORDER BY " + string.Join(", ", OrderByManager.OrderByColumns.Select(o => o.Expression)));

            if (pageNumber > 0 && recordsCount > 0)
                query.Append($" {(OrderByManager.OrderByColumns?.Count > 0 ? "" : "order by (select null)")} OFFSET {pageNumber * recordsCount} ROWS FETCH NEXT {recordsCount} ROWS ONLY");

            using var command = new MySqlCommand(query.ToString(), connection);
            if (parameters.Count > 0)
                command.Parameters.AddRange(parameters.ToArray());

            using var dataReader = command.ExecuteReader();
            MySqlDtoMapper<T> dtoMapper = new(FieldsManager.FieldsList);

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

    public MySqlRawSelectQuery<T> Where(Action<MySqlConditionsManager<T>> fn)
    {
        fn(ConditionsManager);
        return this;
    }

    public MySqlRawSelectQuery<T> OrderBy(Action<OrderByManager<T>> fn)
    {
        fn(OrderByManager);
        return this;
    }

    public MySqlRawSelectQuery<T> Returns(Action<MySqlFieldsSelectionManager<T>> fn)
    {
        fn(FieldsManager);
        return this;
    }

    IRawSelectQuery<T> IRawSelectQuery<T>.OrderBy(Action<OrderByManager<T>> fn) => OrderBy(fn);

    IRawSelectQuery<T> IRawSelectQuery<T>.Returns(Action<IFieldsSelectionManager<T>> fn) => Returns(fn);

    IRawSelectQuery<T> IRawSelectQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);

    IEnumerable<T> IRawSelectQuery<T>.Execute(DbConnection? sqlConnection, int pageNumber, int recordsCount) => Execute((MySqlConnection?)sqlConnection, pageNumber, recordsCount);
}