using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace LazurdIT.FluentOrm.Oracle
{
    public class OracleUpdateQuery<T> : IUpdateQuery<T>
        where T : IFluentModel, new()
    {
        public OracleConditionsManager<T> ConditionsManager { get; } = new();
        public OracleFluentUpdateCriteriaManager<T> UpdateManager { get; } = new();

        public string TableName { get; set; } = OracleDtoMapper<T>.GetTableName();

        public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

        public string? TablePrefix { get; set; }

        ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        public OracleUpdateQuery<T> WithPrefix(string tablePrefix)
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

        public OracleUpdateQuery<T> WithConnection(OracleConnection? connection)
        {
            this.Connection = connection;
            return this;
        }

        public string ExpressionSymbol => ":";

        public OracleConnection? Connection { get; set; }

        FluentUpdateCriteriaManager<T> IUpdateQuery<T>.UpdateManager => this.UpdateManager;

        IFluentConditionsManager<T> IConditionQuery<T>.ConditionsManager => this.ConditionsManager;

        public OracleUpdateQuery(OracleConnection? connection = null)
        {
            Connection = connection;
        }

        public int Execute(
            T record,
            OracleConnection? connection = null,
            bool ignoreEmptyConditions = false
        ) => Execute(record, ConditionsManager, connection, ignoreEmptyConditions);

        public int Execute(
            T record,
            Action<OracleConditionsManager<T>> conditionsFn,
            OracleConnection? connection = null,
            bool ignoreEmptyConditions = false
        )
        {
            OracleConditionsManager<T> conditionsManager = new();
            conditionsFn(conditionsManager);
            return Execute(record, conditionsManager, connection, ignoreEmptyConditions);
        }

        public int Execute(
            T record,
            OracleConditionsManager<T> manager,
            OracleConnection? connection = null,
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
                    UpdateManager.GetFinalExpressions(parameterName, ":")
                );

                StringBuilder updateQuery = new($@"update {TableNameWithPrefix} set {expressions}");

                var parameters = new List<OracleParameter>();

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
                            condition.SetExpressionSymbol(ExpressionSymbol).GetDbParameters().ToNativeDbParameters<OracleParameter>()!
                        );
                    }
                }
                else if (parameters.Count == 0 && !ignoreEmptyConditions)
                {
                    var whereParameter = $"Wh_{parameterName}";
                    OracleDtoMapper<T> dtoMapper = new();
                    parameters.AddRange(
                        dtoMapper.GetPrimaryKeySqlParameters(record, whereParameter).ToList()
                    );
                    if (parameters.Count == 0)
                        throw new Exception("NoConditionsPassedAndCannotDeterminePK");
                    updateQuery.Append(
                        $" where {dtoMapper.GetPrimaryKeySqlWhereString(whereParameter)}"
                    );
                }

                //Add initial parameters
                var initialParameters = UpdateManager.GetSqlParameters(record, parameterName)?.ToList();
                if ((initialParameters?.Count ?? 0) > 0)
                    parameters.AddRange(initialParameters!);

                string query = updateQuery.ToString();
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

        public int Execute(OracleConnection? connection = null, bool ignoreEmptyConditions = false) =>
            Execute(ConditionsManager, connection, ignoreEmptyConditions);

        public int Execute(
            Action<OracleConditionsManager<T>> conditionsFn,
            OracleConnection? connection = null,
            bool ignoreEmptyConditions = false
        )
        {
            OracleConditionsManager<T> conditionsManager = new();
            conditionsFn(conditionsManager);
            return Execute(conditionsManager, connection, ignoreEmptyConditions);
        }

        public int Execute(
            OracleConditionsManager<T> manager,
            OracleConnection? connection = null,
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
                    UpdateManager.GetFinalExpressions(parameterName, ":")
                );

                StringBuilder updateQuery = new($@"update {TableNameWithPrefix} set {expressions}");

                var parameters = new List<OracleParameter>();

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
                            condition.SetExpressionSymbol(ExpressionSymbol).GetDbParameters().ToNativeDbParameters<OracleParameter>()!
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

        public OracleUpdateQuery<T> Where(Action<OracleConditionsManager<T>> fn)
        {
            fn(ConditionsManager);
            return this;
        }

        public OracleUpdateQuery<T> WithFields(Action<FluentUpdateCriteriaManager<T>> fn)
        {
            fn(UpdateManager);
            return this;
        }

        int IUpdateQuery<T>.Execute(T record, DbConnection? connection, bool ignoreEmptyConditions) =>
            Execute(record, (OracleConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            T record,
            Action<IFluentConditionsManager<T>> conditionsFn,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) => Execute(record, conditionsFn, (OracleConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            T record,
            IFluentConditionsManager<T> manager,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) =>
            Execute(
                record,
                (OracleConditionsManager<T>)manager,
                (OracleConnection?)connection,
                ignoreEmptyConditions
            );

        int IUpdateQuery<T>.Execute(DbConnection? connection, bool ignoreEmptyConditions) =>
            Execute((OracleConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            Action<IFluentConditionsManager<T>> conditionsFn,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) => Execute(conditionsFn, (OracleConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            IFluentConditionsManager<T> manager,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) =>
            Execute(
                (OracleConditionsManager<T>)manager,
                (OracleConnection?)connection,
                ignoreEmptyConditions
            );

        IUpdateQuery<T> IUpdateQuery<T>.WithFields(Action<FluentUpdateCriteriaManager<T>> fn) =>
            WithFields(fn);

        IUpdateQuery<T> IUpdateQuery<T>.Where(Action<IFluentConditionsManager<T>> fn) => Where(fn);
    }
}