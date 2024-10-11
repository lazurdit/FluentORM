using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using LazurdIT.FluentOrm.Common;
using MySqlConnector;

namespace LazurdIT.FluentOrm.MySql;

public class MySqlSelectQuery<T> : ISelectQuery<T>
    where T : IFluentModel, new()
{
    public MySqlConditionsManager<T> ConditionsManager { get; } = new();
    public OrderByManager<T> OrderByManager { get; } = new();
    public MySqlFieldsSelectionManager<T> FieldsManager { get; } = new();

    public string TableName { get; set; } = MySqlDtoMapper<T>.GetTableName();

    public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

    public string TablePrefix { get; set; } = string.Empty;

    ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
    {
        this.TablePrefix = tablePrefix;
        return this;
    }

    public MySqlSelectQuery<T> WithPrefix(string tablePrefix)
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

    public MySqlSelectQuery<T> WithConnection(MySqlConnection? connection)
    {
        this.Connection = connection;
        return this;
    }

    public string ExpressionSymbol => "@";

    public MySqlConnection? Connection { get; set; }

    IConditionsManager<T> IConditionQuery<T>.ConditionsManager => ConditionsManager;

    IFieldsSelectionManager<T> ISelectQuery<T>.FieldsManager => FieldsManager;

    public MySqlSelectQuery(MySqlConnection? connection = null)
    {
        Connection = connection;
    }

    public IEnumerable<T> Execute(
        MySqlConnection? connection = null,
        int pageNumber = 0,
        int recordsCount = 0
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
        string includeColumns =
            FieldsManager.FieldsList.Count > 0
                ? string.Join(",", FieldsManager.FieldsList.GetFinalPropertyNames())
                : "*";

        try
        {
            StringBuilder query =
                new(
                    $"SELECT {(recordsCount > 0 && pageNumber <= 0 ? $"TOP {recordsCount}" : "")} {includeColumns} FROM {TableNameWithPrefix}"
                );
            var parameters = new List<MySqlParameter>();

            if (ConditionsManager.WhereConditions.Count > 0)
            {
                int i = 0;
                query.Append(
                    " WHERE "
                        + string.Join(
                            " AND ",
                            ConditionsManager.WhereConditions.Select(w =>
                                w.SetParameterName($"param_{++i}").GetExpression(ExpressionSymbol)
                            )
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

            if (OrderByManager.OrderByColumns?.Count > 0)
                query.Append(
                    " ORDER BY "
                        + string.Join(", ", OrderByManager.OrderByColumns.Select(o => o.Expression))
                );

            if (pageNumber > 0 && recordsCount > 0)
                query.Append(
                    $" {(OrderByManager.OrderByColumns?.Count > 0 ? "" : "order by (select null)")} OFFSET {pageNumber * recordsCount} ROWS FETCH NEXT {recordsCount} ROWS ONLY"
                );

            using var command = new MySqlCommand(query.ToString(), dbConnection);
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
                dbConnection.Close();
        }
    }

    public MySqlSelectQuery<T> Where(Action<MySqlConditionsManager<T>> fn)
    {
        fn(ConditionsManager);
        return this;
    }

    public MySqlSelectQuery<T> OrderBy(Action<OrderByManager<T>> fn)
    {
        fn(OrderByManager);
        return this;
    }

    public MySqlSelectQuery<T> Returns(Action<MySqlFieldsSelectionManager<T>> fn)
    {
        fn(FieldsManager);
        return this;
    }

    IEnumerable<T> ISelectQuery<T>.Execute(
        DbConnection? connection,
        int pageNumber,
        int recordsCount
    ) => Execute((MySqlConnection?)connection, pageNumber, recordsCount);

    ISelectQuery<T> ISelectQuery<T>.OrderBy(Action<OrderByManager<T>> fn) => OrderBy(fn);

    ISelectQuery<T> ISelectQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);

    ISelectQuery<T> ISelectQuery<T>.Returns(Action<IFieldsSelectionManager<T>> fn) => Returns(fn);
}