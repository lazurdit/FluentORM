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
    public class SQLiteUpdateQuery<T> : IUpdateQuery<T>
        where T : IFluentModel, new()
    {
        public SQLiteConditionsManager<T> ConditionsManager { get; } = new();
        public SQLiteFluentUpdateCriteriaManager<T> UpdateManager { get; } = new();

        public string TableName { get; set; } = SQLiteDtoMapper<T>.GetTableName();

        public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

        public string? TablePrefix { get; set; }

        ITableRelatedFluentQuery ITableRelatedFluentQuery.WithPrefix(string tablePrefix)
        {
            this.TablePrefix = tablePrefix;
            return this;
        }

        public SQLiteUpdateQuery<T> WithPrefix(string tablePrefix)
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

        public SQLiteUpdateQuery<T> WithConnection(SQLiteConnection? connection)
        {
            this.Connection = connection;
            return this;
        }

        public string ExpressionSymbol => "@";

        public SQLiteConnection? Connection { get; set; }

        FluentUpdateCriteriaManager<T> IUpdateQuery<T>.UpdateManager => this.UpdateManager;

        IFluentConditionsManager<T> IConditionQuery<T>.ConditionsManager => this.ConditionsManager;

        public SQLiteUpdateQuery(SQLiteConnection? connection = null)
        {
            Connection = connection;
        }

        public int Execute(
            T record,
            SQLiteConnection? connection = null,
            bool ignoreEmptyConditions = false
        ) => Execute(record, ConditionsManager, connection, ignoreEmptyConditions);

        public int Execute(
            T record,
            Action<SQLiteConditionsManager<T>> conditionsFn,
            SQLiteConnection? connection = null,
            bool ignoreEmptyConditions = false
        )
        {
            SQLiteConditionsManager<T> conditionsManager = new();
            conditionsFn(conditionsManager);
            return Execute(record, conditionsManager, connection, ignoreEmptyConditions);
        }

        public int Execute(
            T record,
            SQLiteConditionsManager<T> manager,
            SQLiteConnection? connection = null,
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

                var parameters = new List<SQLiteParameter>();

                if (manager.WhereConditions.Count > 0)
                {
                    int i = 0;
                    updateQuery.Append(
                        " WHERE "
                            + string.Join(
                                " AND ",
                                manager.WhereConditions.Select(w =>
                                    w.SetParameterName($"Wh_param_{++i}")
                                    .SetExpressionSymbol(ExpressionSymbol)
                                        .GetExpression()
                                )
                            )
                    );

                    foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                    {
                        parameters.AddRange(
                            condition.SetExpressionSymbol(ExpressionSymbol).GetDbParameters().ToNativeDbParameters<SQLiteParameter>()!
                        );
                    }
                }
                else if (parameters.Count == 0 && !ignoreEmptyConditions)
                {
                    var whereParameter = $"Wh_{parameterName}";
                    SQLiteDtoMapper<T> dtoMapper = new();
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

        public int Execute(SQLiteConnection? connection = null, bool ignoreEmptyConditions = false) =>
            Execute(ConditionsManager, connection, ignoreEmptyConditions);

        public int Execute(
            Action<SQLiteConditionsManager<T>> conditionsFn,
            SQLiteConnection? connection = null,
            bool ignoreEmptyConditions = false
        )
        {
            SQLiteConditionsManager<T> conditionsManager = new();
            conditionsFn(conditionsManager);
            return Execute(conditionsManager, connection, ignoreEmptyConditions);
        }

        public int Execute(
            SQLiteConditionsManager<T> manager,
            SQLiteConnection? connection = null,
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

                var parameters = new List<SQLiteParameter>();

                if (manager.WhereConditions.Count > 0)
                {
                    int i = 0;
                    updateQuery.Append(
                        " WHERE "
                            + string.Join(
                                " AND ",
                                manager.WhereConditions.Select(w =>
                                    w.SetParameterName($"Wh_param_{++i}")
                                        .SetExpressionSymbol(ExpressionSymbol)
                                        .GetExpression()
                                )
                            )
                    );

                    foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                    {
                        parameters.AddRange(
                            condition.SetExpressionSymbol(ExpressionSymbol).GetDbParameters().ToNativeDbParameters<SQLiteParameter>()!
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

        public SQLiteUpdateQuery<T> Where(Action<SQLiteConditionsManager<T>> fn)
        {
            fn(ConditionsManager);
            return this;
        }

        public SQLiteUpdateQuery<T> WithFields(Action<FluentUpdateCriteriaManager<T>> fn)
        {
            fn(UpdateManager);
            return this;
        }

        int IUpdateQuery<T>.Execute(T record, DbConnection? connection, bool ignoreEmptyConditions) =>
            Execute(record, (SQLiteConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            T record,
            Action<IFluentConditionsManager<T>> conditionsFn,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) => Execute(record, conditionsFn, (SQLiteConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            T record,
            IFluentConditionsManager<T> manager,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) =>
            Execute(
                record,
                (SQLiteConditionsManager<T>)manager,
                (SQLiteConnection?)connection,
                ignoreEmptyConditions
            );

        int IUpdateQuery<T>.Execute(DbConnection? connection, bool ignoreEmptyConditions) =>
            Execute((SQLiteConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            Action<IFluentConditionsManager<T>> conditionsFn,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) => Execute(conditionsFn, (SQLiteConnection?)connection, ignoreEmptyConditions);

        int IUpdateQuery<T>.Execute(
            IFluentConditionsManager<T> manager,
            DbConnection? connection,
            bool ignoreEmptyConditions
        ) =>
            Execute(
                (SQLiteConditionsManager<T>)manager,
                (SQLiteConnection?)connection,
                ignoreEmptyConditions
            );

        IUpdateQuery<T> IUpdateQuery<T>.WithFields(Action<FluentUpdateCriteriaManager<T>> fn) =>
            WithFields(fn);

        IUpdateQuery<T> IUpdateQuery<T>.Where(Action<IFluentConditionsManager<T>> fn) => Where(fn);
    }
}