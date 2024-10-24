using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteSelectQuery<T> : ISelectQuery<T>
        where T : IFluentModel, new()
    {
        public SQLiteConditionsManager<T> ConditionsManager { get; } = new();
        public OrderByManager<T> OrderByManager { get; } = new();
        public SQLiteFieldsSelectionManager<T> FieldsManager { get; } = new();

        public string TableName { get; set; } = SQLiteDtoMapper<T>.GetTableName();

        public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

        public string? TablePrefix { get; set; }

        ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        public SQLiteSelectQuery<T> WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        IDbConnection? IFluentQuery.Connection
        {
            get => Connection;
            set => Connection = (SQLiteConnection?)value;
        }

        IFluentQuery IFluentQuery.WithConnection(IDbConnection? connection)
        {
            this.Connection = (SQLiteConnection?)connection;
            return this;
        }

        public SQLiteSelectQuery<T> WithConnection(SQLiteConnection? connection)
        {
            this.Connection = connection;
            return this;
        }

        public string ExpressionSymbol => "@";

        public SQLiteConnection? Connection { get; set; }

        IConditionsManager<T> IConditionQuery<T>.ConditionsManager => ConditionsManager;

        IFieldsSelectionManager<T> ISelectQuery<T>.FieldsManager => FieldsManager;

        public SQLiteSelectQuery(SQLiteConnection? connection = null)
        {
            Connection = connection;
        }

        public IEnumerable<T> Execute(
            SQLiteConnection? connection = null,
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
                var parameters = new List<SQLiteParameter>();

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
                            (SQLiteParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
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

                using var command = new SQLiteCommand(query.ToString(), dbConnection);
                if (parameters.Count > 0)
                    command.Parameters.AddRange(parameters.ToArray());

                using var dataReader = command.ExecuteReader();

                SQLiteDtoMapper<T> dtoMapper = new(FieldsManager.FieldsList);

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

        public SQLiteSelectQuery<T> Where(Action<SQLiteConditionsManager<T>> fn)
        {
            fn(ConditionsManager);
            return this;
        }

        public SQLiteSelectQuery<T> OrderBy(Action<OrderByManager<T>> fn)
        {
            fn(OrderByManager);
            return this;
        }

        public SQLiteSelectQuery<T> Returns(Action<SQLiteFieldsSelectionManager<T>> fn)
        {
            fn(FieldsManager);
            return this;
        }

        IEnumerable<T> ISelectQuery<T>.Execute(
            DbConnection? connection,
            int pageNumber,
            int recordsCount
        ) => Execute((SQLiteConnection?)connection, pageNumber, recordsCount);

        ISelectQuery<T> ISelectQuery<T>.OrderBy(Action<OrderByManager<T>> fn) => OrderBy(fn);

        ISelectQuery<T> ISelectQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);

        ISelectQuery<T> ISelectQuery<T>.Returns(Action<IFieldsSelectionManager<T>> fn) => Returns(fn);
    }
}