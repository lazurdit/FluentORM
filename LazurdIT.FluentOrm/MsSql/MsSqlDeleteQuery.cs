using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlDeleteQuery<T> : IDeleteQuery<T>
        where T : IFluentModel, new()
    {
        public MsSqlConditionsManager<T> ConditionsManager { get; } = new();

        public string TableName { get; set; } = MsSqlDtoMapper<T>.GetTableName();

        public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

        public string? TablePrefix { get; set; }

        ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        public MsSqlDeleteQuery<T> WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        IDbConnection? IFluentQuery.Connection
        {
            get => Connection;
            set => Connection = (SqlConnection?)value;
        }

        IFluentQuery IFluentQuery.WithConnection(IDbConnection? connection)
        {
            this.Connection = (SqlConnection?)connection;
            return this;
        }

        public MsSqlDeleteQuery<T> WithConnection(SqlConnection? connection)
        {
            this.Connection = connection;
            return this;
        }

        public string ExpressionSymbol => "@";

        public SqlConnection? Connection { get; set; }

        IConditionsManager<T> IConditionQuery<T>.ConditionsManager => this.ConditionsManager;

        string ITableRelatedFluentQuery.TableName
        {
            get => this.TableName;
            set => this.TableName = value;
        }

        public MsSqlDeleteQuery(SqlConnection? connection = null)
        {
            Connection = connection;
        }

        public int Execute(SqlConnection? connection = null, bool deleteAll = false)
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
                var parameters = new List<SqlParameter>();

                if (ConditionsManager.WhereConditions.Count > 0)
                {
                    int i = 0;
                    query +=
                        " WHERE "
                        + string.Join(
                            " AND ",
                            ConditionsManager.WhereConditions.Select(w =>
                                w.SetParameterName($"param_{++i}").GetExpression(ExpressionSymbol)
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

                using var command = new SqlCommand(query, dbConnection);
                if (parameters.Count > 0)
                    command.Parameters.AddRange(parameters.ToArray());

                int count = command.ExecuteNonQuery();
                return count;
            }
            finally
            {
                if (shouldCloseConnection)
                    dbConnection.Close();
            }
        }

        public MsSqlDeleteQuery<T> Where(Action<MsSqlConditionsManager<T>> fn)
        {
            fn(ConditionsManager);
            return this;
        }

        int IDeleteQuery<T>.Execute(DbConnection? connection, bool deleteAll) =>
            Execute((SqlConnection?)connection, deleteAll);

        IDeleteQuery<T> IDeleteQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);
    }
}