using LazurdIT.FluentOrm.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlDeleteQuery<T> : IConditionQuery<T>, IDeleteQuery<T>
        where T : IFluentModel, new()
    {
        public PgsqlDeleteQuery(NpgsqlConnection? connection = null)
        {
            Connection = connection;
        }

        public PgsqlConditionsManager<T> ConditionsManager { get; } = new();

        IFluentConditionsManager<T> IConditionQuery<T>.ConditionsManager => ConditionsManager;
        public NpgsqlConnection? Connection { get; set; }

        IDbConnection? IFluentQuery.Connection
        {
            get => Connection;
            set => Connection = (NpgsqlConnection?)value;
        }

        public string ExpressionSymbol => "@";
        public string TableName { get; set; } = PgsqlDtoMapper<T>.GetTableName();

        public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

        public string? TablePrefix { get; set; }

        public int Execute(NpgsqlConnection? connection = null, bool deleteAll = false)
        {
            // Use the provided connection or the default one
            var dbConnection = connection ?? Connection ?? throw new Exception("ConnectionNotPassed");
            if (!deleteAll && ConditionsManager.WhereConditions.Count == 0)
                throw new Exception("DeleteAllMustBeTrueIfNoWhereConditionPassed");

            // Ensure the connection is open
            var shouldCloseConnection = dbConnection!.State == ConnectionState.Closed;
            if (shouldCloseConnection)
            {
                dbConnection.Open();
            }

            try
            {
                var query = $"delete from {TableNameWithPrefix}";
                var parameters = new List<NpgsqlParameter>();

                if (ConditionsManager.WhereConditions.Count > 0)
                {
                    int i = 0;
                    query +=
                        " WHERE "
                        + string.Join(
                            " AND ",
                            ConditionsManager.WhereConditions.Select(w =>
                                w.SetParameterName($"param_{++i}").SetExpressionSymbol(ExpressionSymbol).GetExpression()
                            )
                        );

                    foreach (
                        var condition in ConditionsManager.WhereConditions.Where(w => w.HasParameters)
                    )
                    {
                        parameters.AddRange(
                            condition.SetExpressionSymbol(ExpressionSymbol).GetDbParameters().ToNativeDbParameters<NpgsqlParameter>()!
                        );
                    }
                }

                using var command = new NpgsqlCommand(query, dbConnection);
                if (parameters.Count > 0)
                    foreach (var parameter in parameters)
                        command.Parameters.Add(parameter);

                int count = command.ExecuteNonQuery();
                return count;
            }
            finally
            {
                if (shouldCloseConnection)
                    dbConnection.Close();
            }
        }

        int IDeleteQuery<T>.Execute(DbConnection? connection, bool deleteAll) =>
            Execute((NpgsqlConnection?)connection, deleteAll);

        public PgsqlDeleteQuery<T> Where(Action<PgsqlConditionsManager<T>> fn)
        {
            fn(ConditionsManager);
            return this;
        }

        IDeleteQuery<T> IDeleteQuery<T>.Where(Action<IFluentConditionsManager<T>> fn) => Where(fn);

        IFluentQuery IFluentQuery.WithConnection(IDbConnection? connection)
        {
            this.Connection = (NpgsqlConnection?)connection;
            return this;
        }

        public PgsqlDeleteQuery<T> WithConnection(NpgsqlConnection? connection)
        {
            this.Connection = connection;
            return this;
        }

        ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        public PgsqlDeleteQuery<T> WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }
    }
}