using LazurdIT.FluentOrm.Common;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace LazurdIT.FluentOrm.Pgsql;

public class PgsqlAggregateSelectQuery<T, ResultType> : IAggregateSelectQuery<T, ResultType> where T : IFluentModel, new()
    where ResultType : IFluentModel, new()
{
    public PgsqlConditionsManager<T> ConditionsManager { get; } = new();
    public PgsqlFieldsSelectionManager<T> GroupByFieldsManager { get; } = new();
    public AggregateFieldsSelectionManager<T> AggregatesManager { get; } = new();
    public AggregateOrderByManager<T> OrderByManager { get; } = new();
    public PgsqlHavingConditionsManager<T> HavingConditionsManager { get; } = new();

    public string ExpressionSymbol => "@";
    public string TableName { get; set; } = PgsqlDtoMapper<T>.GetTableName();

    public NpgsqlConnection? SqlConnection { get; set; }

    DbConnection? IAggregateSelectQuery<T, ResultType>.Connection => SqlConnection;

    IConditionsManager<T> IConditionQuery<T>.ConditionsManager => ConditionsManager;

    IHavingConditionsManager<T> IAggregateSelectQuery<T, ResultType>.HavingConditionsManager => HavingConditionsManager;

    IFieldsSelectionManager<T> IAggregateSelectQuery<T, ResultType>.GroupByFieldsManager => GroupByFieldsManager;

    public PgsqlAggregateSelectQuery(NpgsqlConnection? sqlConnection = null)
    {
        SqlConnection = sqlConnection;
        GroupByFieldsManager.FieldsList.Clear();
    }

    public IEnumerable<ResultType> Execute(NpgsqlConnection? sqlConnection = null, int pageNumber = 0, int recordsCount = 0)
    {
        // Use the provided connection or the default one
        var connection = sqlConnection ?? SqlConnection ?? throw new Exception("ConnectionNotPassed");

        // Ensure the connection is open
        var shouldCloseConnection = connection!.State == ConnectionState.Closed;
        if (shouldCloseConnection)
            connection.Open();

        List<string> headerColumns = new();
        if (AggregatesManager.FieldsList?.Count > 0)
            headerColumns.AddRange(AggregatesManager.FieldsList.GetFinalHeaderStrings());

        if (GroupByFieldsManager.FieldsList?.Count > 0)
            headerColumns.AddRange(GroupByFieldsManager.FieldsList.GetFinalPropertyNames());

        string includeColumns = headerColumns.Count == 0 ? "count(*) records_count" : string.Join(",", headerColumns);

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

            if (GroupByFieldsManager.FieldsList?.Count > 0)
                query.Append($" Group BY {string.Join(", ", GroupByFieldsManager.FieldsList.GetFinalPropertyNames())}");

            if (HavingConditionsManager.HavingConditions.Count > 0)
            {
                query.Append(" HAVING " + string.Join(" AND ", HavingConditionsManager.HavingConditions.Select(w => w.GetExpression(ExpressionSymbol))));

                foreach (var condition in HavingConditionsManager.HavingConditions.Where(w => w.HasParameters))
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

            PgsqlDtoMapper<ResultType> dtoMapper = new();

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

    public PgsqlAggregateSelectQuery<T, ResultType> Where(Action<PgsqlConditionsManager<T>> fn)
    {
        fn(ConditionsManager);
        return this;
    }

    public PgsqlAggregateSelectQuery<T, ResultType> OrderBy(Action<AggregateOrderByManager<T>> fn)
    {
        fn(OrderByManager);
        return this;
    }

    public PgsqlAggregateSelectQuery<T, ResultType> Aggregate(Action<AggregateFieldsSelectionManager<T>> fn)
    {
        fn(AggregatesManager);
        return this;
    }

    public PgsqlAggregateSelectQuery<T, ResultType> GroupBy(Action<PgsqlFieldsSelectionManager<T>> fn)
    {
        fn(GroupByFieldsManager);
        return this;
    }

    public PgsqlAggregateSelectQuery<T, ResultType> Having(Action<PgsqlHavingConditionsManager<T>> fn)
    {
        fn(HavingConditionsManager);
        return this;
    }

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.Aggregate(Action<AggregateFieldsSelectionManager<T>> fn) => Aggregate(fn);

    IEnumerable<ResultType> IAggregateSelectQuery<T, ResultType>.Execute(DbConnection? sqlConnection, int pageNumber, int recordsCount) => Execute((NpgsqlConnection?)sqlConnection, pageNumber, recordsCount);

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.GroupBy(Action<IFieldsSelectionManager<T>> fn) => GroupBy(fn);

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.OrderBy(Action<AggregateOrderByManager<T>> fn) => OrderBy(fn);

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.Where(Action<IConditionsManager<T>> fn) => Where(fn);

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.Having(Action<IHavingConditionsManager<T>> fn) => Having(fn);
}