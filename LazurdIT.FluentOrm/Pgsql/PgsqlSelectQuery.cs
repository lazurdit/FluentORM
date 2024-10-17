﻿using LazurdIT.FluentOrm.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlSelectQuery<T> : ISelectQuery<T>
        where T : IFluentModel, new()
    {
        public PgsqlConditionsManager<T> ConditionsManager { get; } = new();
        public OrderByManager<T> OrderByManager { get; } = new();
        public PgsqlFieldsSelectionManager<T> FieldsManager { get; } = new();

        public string TableName { get; set; } = PgsqlDtoMapper<T>.GetTableName();

        public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

        public string? TablePrefix { get; set; } 

        ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        public PgsqlSelectQuery<T> WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        IDbConnection? IFluentQuery.Connection
        {
            get => Connection;
            set => Connection = (NpgsqlConnection?)value;
        }

        IFluentQuery IFluentQuery.WithConnection(IDbConnection? connection)
        {
            this.Connection = (NpgsqlConnection?)connection;
            return this;
        }

        public PgsqlSelectQuery<T> WithConnection(NpgsqlConnection? connection)
        {
            this.Connection = connection;
            return this;
        }

        public string ExpressionSymbol => "@";

        public NpgsqlConnection? Connection { get; set; }

        IConditionsManager<T> IConditionQuery<T>.ConditionsManager => ConditionsManager;

        IFieldsSelectionManager<T> ISelectQuery<T>.FieldsManager => FieldsManager;

        public PgsqlSelectQuery(NpgsqlConnection? connection = null)
        {
            Connection = connection;
        }

        public IEnumerable<T> Execute(
            NpgsqlConnection? connection = null,
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
                var parameters = new List<SqlParameter>();

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
                            (SqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
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

                using var command = new NpgsqlCommand(query.ToString(), dbConnection);
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
                    dbConnection.Close();
            }
        }

        public PgsqlSelectQuery<T> Where(Action<PgsqlConditionsManager<T>> fn) => Where(fn);

        public PgsqlSelectQuery<T> OrderBy(Action<OrderByManager<T>> fn) => OrderBy(fn);

        public PgsqlSelectQuery<T> Returns(Action<PgsqlFieldsSelectionManager<T>> fn) => Returns(fn);

        IEnumerable<T> ISelectQuery<T>.Execute(
            DbConnection? connection,
            int pageNumber,
            int recordsCount
        ) => Execute((NpgsqlConnection?)connection, pageNumber, recordsCount);

        ISelectQuery<T> ISelectQuery<T>.OrderBy(Action<OrderByManager<T>> fn) => OrderBy(fn);

        ISelectQuery<T> ISelectQuery<T>.Returns(Action<IFieldsSelectionManager<T>> fn) => Returns(fn);

        ISelectQuery<T> ISelectQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);
    }
}