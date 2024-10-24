using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteDeleteQuery<T> : IDeleteQuery<T>
        where T : IFluentModel, new()
    {
        public SQLiteConditionsManager<T> ConditionsManager { get; } = new();

        public string TableName { get; set; } = SQLiteDtoMapper<T>.GetTableName();

        public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

        public string? TablePrefix { get; set; }

        ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        public SQLiteDeleteQuery<T> WithPrefix(string tablePrefix)
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

        public SQLiteDeleteQuery<T> WithConnection(SQLiteConnection? connection)
        {
            this.Connection = connection;
            return this;
        }

        public string ExpressionSymbol => "@";

        public SQLiteConnection? Connection { get; set; }

        IConditionsManager<T> IConditionQuery<T>.ConditionsManager => this.ConditionsManager;

        string ITableRelatedFluentQuery.TableName
        {
            get => this.TableName;
            set => this.TableName = value;
        }

        public SQLiteDeleteQuery(SQLiteConnection? connection = null)
        {
            Connection = connection;
        }

        public int Execute(SQLiteConnection? connection = null, bool deleteAll = false)
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
                var parameters = new List<SQLiteParameter>();

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
                            (SQLiteParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
                        );
                    }
                }

                using var command = new SQLiteCommand(query, dbConnection);
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

        public SQLiteDeleteQuery<T> Where(Action<SQLiteConditionsManager<T>> fn)
        {
            fn(ConditionsManager);
            return this;
        }

        int IDeleteQuery<T>.Execute(DbConnection? connection, bool deleteAll) =>
            Execute((SQLiteConnection?)connection, deleteAll);

        IDeleteQuery<T> IDeleteQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);
    }
}