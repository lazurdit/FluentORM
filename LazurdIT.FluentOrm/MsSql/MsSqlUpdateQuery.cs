using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlUpdateQuery<T> : IUpdateQuery<T>
        where T : IFluentModel, new()
    {
        public MsSqlConditionsManager<T> ConditionsManager { get; } = new();
        public MsSqlFluentUpdateCriteriaManager<T> UpdateManager { get; } = new();

        public string TableName { get; set; } = MsSqlDtoMapper<T>.GetTableName();

        public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

        public string? TablePrefix { get; set; } 

        ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        public MsSqlUpdateQuery<T> WithPrefix(string tablePrefix)
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

        public MsSqlUpdateQuery<T> WithConnection(SqlConnection? connection)
        {
            this.Connection = connection;
            return this;
        }

        public string ExpressionSymbol => "@";

        public SqlConnection? Connection { get; set; }

        FluentUpdateCriteriaManager<T> IUpdateQuery<T>.UpdateManager => this.UpdateManager;

        IConditionsManager<T> IConditionQuery<T>.ConditionsManager => this.ConditionsManager;

        public MsSqlUpdateQuery(SqlConnection? connection = null)
        {
            Connection = connection;
        }

        public int Execute(
            T record,
            SqlConnection? connection = null,
            bool ignoreEmptyConditions = false
        ) => Execute(record, ConditionsManager, connection, ignoreEmptyConditions);

        public int Execute(
            T record,
            Action<MsSqlConditionsManager<T>> conditionsFn,
            SqlConnection? connection = null,
            bool ignoreEmptyConditions = false
        )
        {
            MsSqlConditionsManager<T> conditionsManager = new();
            conditionsFn(conditionsManager);
            return Execute(record, conditionsManager, connection, ignoreEmptyConditions);
        }

        public int Execute(
            T record,
            MsSqlConditionsManager<T> manager,
            SqlConnection? connection = null,
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

                var parameters = new List<SqlParameter>();

                if (manager.WhereConditions.Count > 0)
                {
                    int i = 0;
                    updateQuery.Append(
                        " WHERE "
                            + string.Join(
                                " AND ",
                                manager.WhereConditions.Select(w =>
                                    w.SetParameterName($"Wh_param_{++i}")
                                        .GetExpression(ExpressionSymbol)
                                )
                            )
                    );

                    foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                    {
                        parameters.AddRange(
                            (SqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
                        );
                    }
                }
                else if (parameters.Count == 0 && !ignoreEmptyConditions)
                {
                    var whereParameter = $"Wh_{parameterName}";
                    MsSqlDtoMapper<T> dtoMapper = new();
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

        public int Execute(SqlConnection? connection = null, bool ignoreEmptyConditions = false) =>
            Execute(ConditionsManager, connection, ignoreEmptyConditions);

        public int Execute(
            Action<MsSqlConditionsManager<T>> conditionsFn,
            SqlConnection? connection = null,
            bool ignoreEmptyConditions = false
        )
        {
            MsSqlConditionsManager<T> conditionsManager = new();
            conditionsFn(conditionsManager);
            return Execute(conditionsManager, connection, ignoreEmptyConditions);
        }

        public int Execute(
            MsSqlConditionsManager<T> manager,
            SqlConnection? connection = null,
            bool ignoreEmptyConditions = false
        )
        {
            var dbConnection = connection ?? Connection ?? throw new Exception("ConnectionNotPassed");

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

                var parameters = new List<SqlParameter>();

                if (manager.WhereConditions.Count > 0)
                {
                    int i = 0;
                    updateQuery.Append(
                        " WHERE "
                            + string.Join(
                                " AND ",
                                manager.WhereConditions.Select(w =>
                                    w.SetParameterName($"Wh_param_{++i}")
                                        .GetExpression(ExpressionSymbol)
                                )
                            )
                    );

                    foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                    {
                        parameters.AddRange(
                            (SqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
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

        public MsSqlUpdateQuery<T> Where(Action<MsSqlConditionsManager<T>> fn)
        {
            fn(ConditionsManager);
            return this;
        }

        public MsSqlUpdateQuery<T> WithFields(Action<FluentUpdateCriteriaManager<T>> fn)
        {
            fn(UpdateManager);
            return this;
        }

        int IUpdateQuery<T>.Execute(T record, DbConnection? connection, bool ignoreEmptyConditions) =>
            Execute(record, (SqlConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            T record,
            Action<IConditionsManager<T>> conditionsFn,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) => Execute(record, conditionsFn, (SqlConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            T record,
            IConditionsManager<T> manager,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) =>
            Execute(
                record,
                (MsSqlConditionsManager<T>)manager,
                (SqlConnection?)connection,
                ignoreEmptyConditions
            );

        int IUpdateQuery<T>.Execute(DbConnection? connection, bool ignoreEmptyConditions) =>
            Execute((SqlConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            Action<IConditionsManager<T>> conditionsFn,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) => Execute(conditionsFn, (SqlConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            IConditionsManager<T> manager,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) =>
            Execute(
                (MsSqlConditionsManager<T>)manager,
                (SqlConnection?)connection,
                ignoreEmptyConditions
            );

        IUpdateQuery<T> IUpdateQuery<T>.WithFields(Action<FluentUpdateCriteriaManager<T>> fn) =>
            WithFields(fn);

        IUpdateQuery<T> IUpdateQuery<T>.Where(Action<IConditionsManager<T>> fn) => Where(fn);
    }
}