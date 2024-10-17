using LazurdIT.FluentOrm.Common;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace LazurdIT.FluentOrm.MySql
{
    public class MySqlAggregateSelectQuery<T, ResultType> : IAggregateSelectQuery<T, ResultType>
        where T : IFluentModel, new()
        where ResultType : IFluentModel, new()
    {
        public MySqlConditionsManager<T> ConditionsManager { get; } = new();
        public MySqlFieldsSelectionManager<T> GroupByFieldsManager { get; } = new();
        public AggregateFieldsSelectionManager<T> AggregatesManager { get; } = new();
        public AggregateOrderByManager<T> OrderByManager { get; } = new();
        public MySqlHavingConditionsManager<T> HavingConditionsManager { get; } = new();

        public MySqlConnection? Connection { get; set; }

        IConditionsManager<T> IConditionQuery<T>.ConditionsManager => ConditionsManager;

        IHavingConditionsManager<T> IAggregateSelectQuery<T, ResultType>.HavingConditionsManager =>
            HavingConditionsManager;

        IFieldsSelectionManager<T> IAggregateSelectQuery<T, ResultType>.GroupByFieldsManager =>
            GroupByFieldsManager;

        public string TableName { get; set; } = MySqlDtoMapper<T>.GetTableName();

        public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

        public string? TablePrefix { get; set; } 

        ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        public MySqlAggregateSelectQuery<T, ResultType> WithPrefix(string tablePrefix)
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

        public MySqlAggregateSelectQuery<T, ResultType> WithConnection(MySqlConnection? connection)
        {
            this.Connection = connection;
            return this;
        }

        public string ExpressionSymbol => "@";

        public MySqlAggregateSelectQuery(MySqlConnection? connection = null)
        {
            Connection = connection;
            GroupByFieldsManager.FieldsList.Clear();
        }

        public IEnumerable<ResultType> Execute(
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

                MySqlDtoMapper<ResultType> dtoMapper = new();

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

        public MySqlAggregateSelectQuery<T, ResultType> Where(Action<MySqlConditionsManager<T>> fn)
        {
            fn(ConditionsManager);
            return this;
        }

        public MySqlAggregateSelectQuery<T, ResultType> OrderBy(Action<AggregateOrderByManager<T>> fn)
        {
            fn(OrderByManager);
            return this;
        }

        public MySqlAggregateSelectQuery<T, ResultType> Aggregate(
            Action<AggregateFieldsSelectionManager<T>> fn
        )
        {
            fn(AggregatesManager);
            return this;
        }

        public MySqlAggregateSelectQuery<T, ResultType> GroupBy(
            Action<MySqlFieldsSelectionManager<T>> fn
        )
        {
            fn(GroupByFieldsManager);
            return this;
        }

        public MySqlAggregateSelectQuery<T, ResultType> Having(
            Action<MySqlHavingConditionsManager<T>> fn
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
        ) => Execute((MySqlConnection?)connection, pageNumber, recordsCount);

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
}