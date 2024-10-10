﻿using System.Data;
using System.Data.Common;
using System.Text;
using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;

namespace LazurdIT.FluentOrm.Oracle;

public class OracleAggregateSelectQuery<T, ResultType> : IAggregateSelectQuery<T, ResultType>
    where T : IFluentModel, new()
    where ResultType : IFluentModel, new()
{
    public OracleConditionsManager<T> ConditionsManager { get; } = new();
    public OracleFieldsSelectionManager<T> GroupByFieldsManager { get; } = new();
    public AggregateFieldsSelectionManager<T> AggregatesManager { get; } = new();
    public AggregateOrderByManager<T> OrderByManager { get; } = new();
    public OracleHavingConditionsManager<T> HavingConditionsManager { get; } = new();

    public string TableName { get; set; } = OracleDtoMapper<T>.GetTableName();

    public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

    public string TablePrefix { get; set; } = string.Empty;

    ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
    {
        this.TablePrefix = tablePrefix;
        return this;
    }

    public OracleAggregateSelectQuery<T, ResultType> WithPrefix(string tablePrefix)
    {
        this.TablePrefix = tablePrefix;
        return this;
    }

    IDbConnection? IFluentQuery.Connection
    {
        get => Connection;
        set => Connection = (OracleConnection?)value;
    }

    IFluentQuery IFluentQuery.WithConnection(IDbConnection? connection)
    {
        this.Connection = (OracleConnection?)connection;
        return this;
    }

    public OracleAggregateSelectQuery<T, ResultType> WithConnection(OracleConnection? connection)
    {
        this.Connection = connection;
        return this;
    }

    public string ExpressionSymbol => ":";

    public OracleConnection? Connection { get; set; }

    IConditionsManager<T> IConditionQuery<T>.ConditionsManager => ConditionsManager;

    IHavingConditionsManager<T> IAggregateSelectQuery<T, ResultType>.HavingConditionsManager =>
        HavingConditionsManager;

    IFieldsSelectionManager<T> IAggregateSelectQuery<T, ResultType>.GroupByFieldsManager =>
        GroupByFieldsManager;

    public OracleAggregateSelectQuery(OracleConnection? connection = null)
    {
        Connection = connection;
        GroupByFieldsManager.FieldsList.Clear();
    }

    public IEnumerable<ResultType> Execute(
        OracleConnection? connection = null,
        int pageNumber = 0,
        int recordsCount = 0
    )
    {
        // Use the provided connection or the default one
        var dbConnection = connection ?? Connection ?? throw new Exception("ConnectionNotPassed");

        // Ensure the connection is open
        var shouldCloseConnection = dbConnection!.State == ConnectionState.Closed;
        if (shouldCloseConnection)
            dbConnection.Open();

        List<string> headerColumns = new();
        if (AggregatesManager.FieldsList?.Count > 0)
            headerColumns.AddRange(AggregatesManager.FieldsList.GetFinalHeaderStrings());

        if (GroupByFieldsManager.FieldsList?.Count > 0)
            headerColumns.AddRange(GroupByFieldsManager.FieldsList.GetFinalPropertyNames());

        string includeColumns =
            headerColumns.Count == 0 ? "count(*) records_count" : string.Join(",", headerColumns);

        try
        {
            StringBuilder query =
                new(
                    $"SELECT {(recordsCount > 0 && pageNumber <= 0 ? $"TOP {recordsCount}" : "")} {includeColumns} FROM {TableNameWithPrefix}"
                );
            var parameters = new List<OracleParameter>();

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
                        (OracleParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
                    );
                }
            }

            if (GroupByFieldsManager.FieldsList?.Count > 0)
                query.Append(
                    $" Group BY {string.Join(", ", GroupByFieldsManager.FieldsList.GetFinalPropertyNames())}"
                );

            if (HavingConditionsManager.HavingConditions.Count > 0)
            {
                query.Append(
                    " HAVING "
                        + string.Join(
                            " AND ",
                            HavingConditionsManager.HavingConditions.Select(w =>
                                w.GetExpression(ExpressionSymbol)
                            )
                        )
                );

                foreach (
                    var condition in HavingConditionsManager.HavingConditions.Where(w =>
                        w.HasParameters
                    )
                )
                {
                    parameters.AddRange(
                        (OracleParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
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

            using var command = new OracleCommand(query.ToString(), dbConnection)
            {
                BindByName = true
            };

            if (parameters.Count > 0)
                command.Parameters.AddRange(parameters.ToArray());

            using var dataReader = command.ExecuteReader();

            OracleDtoMapper<ResultType> dtoMapper = new();

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

    public OracleAggregateSelectQuery<T, ResultType> Where(Action<OracleConditionsManager<T>> fn)
    {
        fn(ConditionsManager);
        return this;
    }

    public OracleAggregateSelectQuery<T, ResultType> OrderBy(Action<AggregateOrderByManager<T>> fn)
    {
        fn(OrderByManager);
        return this;
    }

    public OracleAggregateSelectQuery<T, ResultType> Aggregate(
        Action<AggregateFieldsSelectionManager<T>> fn
    )
    {
        fn(AggregatesManager);
        return this;
    }

    public OracleAggregateSelectQuery<T, ResultType> GroupBy(
        Action<OracleFieldsSelectionManager<T>> fn
    )
    {
        fn(GroupByFieldsManager);
        return this;
    }

    public OracleAggregateSelectQuery<T, ResultType> Having(
        Action<OracleHavingConditionsManager<T>> fn
    )
    {
        fn(HavingConditionsManager);
        return this;
    }

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.Aggregate(
        Action<AggregateFieldsSelectionManager<T>> fn
    ) => Aggregate(fn);

    IEnumerable<ResultType> IAggregateSelectQuery<T, ResultType>.Execute(
        DbConnection? connection,
        int pageNumber,
        int recordsCount
    ) => Execute((OracleConnection?)connection, pageNumber, recordsCount);

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.GroupBy(
        Action<IFieldsSelectionManager<T>> fn
    ) => GroupBy(fn);

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.Having(
        Action<IHavingConditionsManager<T>> fn
    ) => Having(fn);

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.OrderBy(
        Action<AggregateOrderByManager<T>> fn
    ) => OrderBy(fn);

    IAggregateSelectQuery<T, ResultType> IAggregateSelectQuery<T, ResultType>.Where(
        Action<IConditionsManager<T>> fn
    ) => Where(fn);
}