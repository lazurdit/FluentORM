using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;

namespace LazurdIT.FluentOrm.Oracle
{
    public class OracleDeleteQuery<T> : IDeleteQuery<T>
        where T : IFluentModel, new()
    {
        public OracleConditionsManager<T> ConditionsManager { get; } = new();

        public string TableName { get; set; } = OracleDtoMapper<T>.GetTableName();

        public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

        public string? TablePrefix { get; set; } 

        ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        public OracleDeleteQuery<T> WithPrefix(string tablePrefix)
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

        public OracleDeleteQuery<T> WithConnection(OracleConnection? connection)
        {
            this.Connection = connection;
            return this;
        }

        public string ExpressionSymbol => ":";

        public OracleConnection? Connection { get; set; }

        IConditionsManager<T> IConditionQuery<T>.ConditionsManager => this.ConditionsManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDeleteQuery{T}"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public OracleDeleteQuery(OracleConnection? connection = null)
        {
            Connection = connection;
        }

        public int Execute(OracleConnection? connection = null, bool deleteAll = false)
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
                var query = $"delete from {TableNameWithPrefix.ToLower()}";
                var parameters = new List<OracleParameter>();

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
                            (OracleParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
                        );
                    }
                }

                using var command = new OracleCommand(query, dbConnection) { BindByName = true };
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

        public OracleDeleteQuery<T> Where(Action<OracleConditionsManager<T>> fn)
        {
            fn(ConditionsManager);
            return this;
        }

        int IDeleteQuery<T>.Execute(DbConnection? connection, bool deleteAll) =>
            Execute((OracleConnection?)connection, deleteAll);

        IDeleteQuery<T> IDeleteQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);
    }
}