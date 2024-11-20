using LazurdIT.FluentOrm.Common;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlUpdateQuery<T> : IUpdateQuery<T>
        where T : IFluentModel, new()
    {
        public PgsqlConditionsManager<T> ConditionsManager { get; } = new();
        public PgsqlFluentUpdateCriteriaManager<T> UpdateManager { get; } = new();

        public string TableName { get; set; } = PgsqlDtoMapper<T>.GetTableName();

        public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

        public string? TablePrefix { get; set; }

        ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        public PgsqlUpdateQuery<T> WithPrefix(string tablePrefix)
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

        public PgsqlUpdateQuery<T> WithConnection(NpgsqlConnection? connection)
        {
            this.Connection = connection;
            return this;
        }

        public string ExpressionSymbol => "@";

        public NpgsqlConnection? Connection { get; set; }

        IFluentConditionsManager<T> IConditionQuery<T>.ConditionsManager => this.ConditionsManager;

        FluentUpdateCriteriaManager<T> IUpdateQuery<T>.UpdateManager => this.UpdateManager;

        public PgsqlUpdateQuery(NpgsqlConnection? connection = null)
        {
            Connection = connection;
        }

        public int Execute(
            T record,
            NpgsqlConnection? connection = null,
            bool ignoreEmptyConditions = false
        ) => Execute(record, ConditionsManager, connection, ignoreEmptyConditions);

        public int Execute(
            T record,
            Action<PgsqlConditionsManager<T>> conditionsFn,
            NpgsqlConnection? connection = null,
            bool ignoreEmptyConditions = false
        )
        {
            PgsqlConditionsManager<T> conditionsManager = new();
            conditionsFn(conditionsManager);
            return Execute(record, conditionsManager, connection, ignoreEmptyConditions);
        }

        public int Execute(
            T record,
            PgsqlConditionsManager<T> manager,
            NpgsqlConnection? connection = null,
            bool ignoreEmptyConditions = false
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
            string parameterName = "P1_";

            try
            {
                if (UpdateManager.Criterias.Count == 0)
                    throw new Exception("NoFieldsToUpdate");

                string expressions = string.Join(
                    ",",
                    UpdateManager.GetFinalExpressions(parameterName, "@")
                );

                StringBuilder updateQuery = new($@"update {TableNameWithPrefix} set {expressions}");

                var parameters = new List<NpgsqlParameter>();

                if (manager.WhereConditions.Count > 0)
                {
                    int i = 0;
                    updateQuery.Append(
                        " WHERE "
                            + string.Join(
                                " AND ",
                                manager.WhereConditions.Select(w =>
                                    w.SetParameterName($"Wh_param_{++i}")
                                        .SetExpressionSymbol(ExpressionSymbol).GetExpression()
                                )
                            )
                    );

                    foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                    {
                        parameters.AddRange(
                            condition.SetExpressionSymbol(ExpressionSymbol).GetDbParameters().ToNativeDbParameters<NpgsqlParameter>()!
                        );
                    }
                }
                else if (parameters.Count == 0 && !ignoreEmptyConditions)
                {
                    var whereParameter = $"Wh_{parameterName}";
                    PgsqlDtoMapper<T> dtoMapper = new();
                    parameters.AddRange(
                        dtoMapper.GetPrimaryKeySqlParameters(record, whereParameter).ToList()
                    );
                    if (parameters.Count == 0)
                        throw new Exception("NoConditionsPassedAndCannotDeterminePK");
                    updateQuery = updateQuery.Append(
                        $" where {dtoMapper.GetPrimaryKeySqlWhereString(whereParameter)}"
                    );
                }

                //Add initial parameters
                var initialParameters = UpdateManager.GetSqlParameters(record, parameterName)?.ToList();

                if ((initialParameters?.Count ?? 0) > 0)
                    parameters.AddRange(initialParameters!);

                string query = updateQuery.ToString();
                using var command = new NpgsqlCommand(query, dbConnection);
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

        public int Execute(NpgsqlConnection? connection = null, bool ignoreEmptyConditions = false) =>
            Execute(ConditionsManager, connection, ignoreEmptyConditions);

        public int Execute(
            Action<PgsqlConditionsManager<T>> conditionsFn,
            NpgsqlConnection? connection = null,
            bool ignoreEmptyConditions = false
        )
        {
            PgsqlConditionsManager<T> conditionsManager = new();
            conditionsFn(conditionsManager);
            return Execute(conditionsManager, connection, ignoreEmptyConditions);
        }

        public int Execute(
            PgsqlConditionsManager<T> manager,
            NpgsqlConnection? connection = null,
            bool ignoreEmptyConditions = false
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
            string parameterName = "P1_";

            try
            {
                if (UpdateManager.Criterias.Count == 0)
                    throw new Exception("NoFieldsToUpdate");

                string expressions = string.Join(
                    ",",
                    UpdateManager.GetFinalExpressions(parameterName, "@")
                );

                StringBuilder updateQuery = new($@"update {TableNameWithPrefix} set {expressions}");

                var parameters = new List<NpgsqlParameter>();

                if (manager.WhereConditions.Count > 0)
                {
                    int i = 0;
                    updateQuery.Append(
                        " WHERE "
                            + string.Join(
                                " AND ",
                                manager.WhereConditions.Select(w =>
                                    w.SetParameterName($"Wh_param_{++i}")
                                        .SetExpressionSymbol(ExpressionSymbol).GetExpression()
                                )
                            )
                    );

                    foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                    {
                        parameters.AddRange(
                            condition.SetExpressionSymbol(ExpressionSymbol).GetDbParameters().ToNativeDbParameters<NpgsqlParameter>()!
                        );
                    }
                }
                else if (parameters.Count == 0 && !ignoreEmptyConditions)
                    throw new Exception("NoConditionsPassedAndCannotDeterminePK");

                //Add initial parameters
                var initialParameters = UpdateManager
                    .GetSqlParameters(default, parameterName)
                    ?.ToList();

                if ((initialParameters?.Count ?? 0) > 0)
                    parameters.AddRange(initialParameters!);

                string query = updateQuery.ToString();
                using var command = new NpgsqlCommand(query, dbConnection);
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

        public PgsqlUpdateQuery<T> Where(Action<PgsqlConditionsManager<T>> fn)
        {
            fn(ConditionsManager);
            return this;
        }

        public PgsqlUpdateQuery<T> WithFields(Action<FluentUpdateCriteriaManager<T>> fn)
        {
            fn(UpdateManager);
            return this;
        }

        int IUpdateQuery<T>.Execute(T record, DbConnection? connection, bool ignoreEmptyConditions) =>
            Execute(record, (NpgsqlConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            T record,
            Action<IFluentConditionsManager<T>> conditionsFn,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) => Execute(record, conditionsFn, (NpgsqlConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            T record,
            IFluentConditionsManager<T> manager,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) =>
            Execute(
                record,
                (PgsqlConditionsManager<T>)manager,
                (NpgsqlConnection?)connection,
                ignoreEmptyConditions
            );

        int IUpdateQuery<T>.Execute(DbConnection? connection, bool ignoreEmptyConditions) =>
            Execute((NpgsqlConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            Action<IFluentConditionsManager<T>> conditionsFn,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) => Execute(conditionsFn, (NpgsqlConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            IFluentConditionsManager<T> manager,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) =>
            Execute(
                (PgsqlConditionsManager<T>)manager,
                (NpgsqlConnection?)connection,
                ignoreEmptyConditions
            );

        IUpdateQuery<T> IUpdateQuery<T>.WithFields(Action<FluentUpdateCriteriaManager<T>> fn) =>
            WithFields(fn);

        IUpdateQuery<T> IUpdateQuery<T>.Where(Action<IFluentConditionsManager<T>> fn) => Where(fn);
    }
}