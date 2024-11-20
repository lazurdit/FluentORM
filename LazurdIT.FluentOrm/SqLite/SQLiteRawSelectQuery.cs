using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteRawSelectQuery<T> : IRawSelectQuery<T>
        where T : IFluentModel, new()
    {
        public SQLiteConditionsManager<T> ConditionsManager { get; } = new();
        public OrderByManager<T> OrderByManager { get; } = new();
        public SQLiteFieldsSelectionManager<T> FieldsManager { get; } = new();

        public string SelectClause { get; set; } = string.Empty;

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

        public SQLiteRawSelectQuery<T> WithConnection(SQLiteConnection? connection)
        {
            this.Connection = connection;
            return this;
        }

        public string ExpressionSymbol => "@";

        public SQLiteConnection? Connection { get; set; }

        IFluentConditionsManager<T> IConditionQuery<T>.ConditionsManager => ConditionsManager;

        IFieldsSelectionManager<T> IRawSelectQuery<T>.FieldsManager => FieldsManager;

        public SQLiteRawSelectQuery(SQLiteConnection? connection = null)
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
                StringBuilder query = new(SelectClause);
                var parameters = new List<SQLiteParameter>();

                if (ConditionsManager.WhereConditions.Count > 0)
                {
                    int i = 0;
                    query.Append(
                        $" WHERE {string.Join(" AND ", ConditionsManager.WhereConditions.Select(w => w.SetParameterName($"param_{++i}").SetExpressionSymbol(ExpressionSymbol).GetExpression()))}"
                    );

                    foreach (
                        var condition in ConditionsManager.WhereConditions.Where(w => w.HasParameters)
                    )
                    {
                        parameters.AddRange(
                            condition.SetExpressionSymbol(ExpressionSymbol).GetDbParameters().ToNativeDbParameters<SQLiteParameter>()!
                        );
                    }
                }

                if (OrderByManager.OrderByColumns?.Count > 0)
                    query.Append(
                        " ORDER BY "
                            + string.Join(", ", OrderByManager.OrderByColumns.Select(o => o.Expression))
                    );

                if (pageNumber >= 0 && recordsCount > 0)
                    query.Append(
                        $" {(OrderByManager.OrderByColumns?.Count > 0 ? "" : "order by (select null)")} Limit  {pageNumber * recordsCount} ,  {recordsCount}"
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

        public SQLiteRawSelectQuery<T> Where(Action<SQLiteConditionsManager<T>> fn)
        {
            fn(ConditionsManager);
            return this;
        }

        public SQLiteRawSelectQuery<T> OrderBy(Action<OrderByManager<T>> fn)
        {
            fn(OrderByManager);
            return this;
        }

        public SQLiteRawSelectQuery<T> Returns(Action<SQLiteFieldsSelectionManager<T>> fn)
        {
            fn(FieldsManager);
            return this;
        }

        IRawSelectQuery<T> IRawSelectQuery<T>.OrderBy(Action<OrderByManager<T>> fn) => OrderBy(fn);

        IRawSelectQuery<T> IRawSelectQuery<T>.Returns(Action<IFieldsSelectionManager<T>> fn) =>
            Returns(fn);

        IRawSelectQuery<T> IRawSelectQuery<T>.Where(Action<IFluentConditionsManager<T>> fn) => Where(fn);

        IEnumerable<T> IRawSelectQuery<T>.Execute(
            DbConnection? connection,
            int pageNumber,
            int recordsCount
        ) => Execute((SQLiteConnection?)connection, pageNumber, recordsCount);
    }
}