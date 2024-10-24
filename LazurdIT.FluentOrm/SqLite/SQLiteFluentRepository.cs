using LazurdIT.FluentOrm.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace LazurdIT.FluentOrm.SQLite
{
    public abstract class SQLiteFluentRepository<T> : IFluentRepository<T>
        where T : IFluentModel, new()
    {
        public List<IFluentRelation> InRelations { get; set; } = new();
        public List<IFluentRelation> OutRelations { get; set; } = new();
        public SQLiteInsertQuery<T>? InsertQuery { get; private set; }
        public SQLiteUpdateQuery<T>? UpdateQuery { get; private set; }
        public SQLiteDeleteQuery<T>? DeleteQuery { get; private set; }
        public SQLiteSelectQuery<T>? SelectQuery { get; private set; }

        public string ExpressionSymbol => "@";

        public SQLiteFluentRepository<T> Build()
        {
            BuildDefaultQueries();
            return this;
        }

        public string TableName { get; set; } = SQLiteDtoMapper<T>.GetTableName();

        public string? TablePrefix { get; set; }

        public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

        private static string GetRelationQuery(IFluentRelation relation)
        {
            StringBuilder queryString =
                new($@"select top 1 1 FROM {relation.TargetTablePrefix}{relation.TargetTableName}");

            StringBuilder relationStringBuilder = new();

            for (int i = 0; i < relation.Fields.Count; i++)
            {
                var field = relation.Fields.ElementAt(i);
                relationStringBuilder.Append(
                    $@"{(i > 0 ? " and " : "")} {field.TargetFieldName.FinalPropertyName} = @{field.SourceFieldName.FinalPropertyName}"
                );
            }
            if (relationStringBuilder.Length > 0)
                queryString.Append($" where {relationStringBuilder}");
            return queryString.ToString();
        }

        public bool IsUsedByAnyOutRelation(
            Func<SQLiteConditionsManager<T>, SQLiteConditionsManager<T>> conditionsManager,
            SQLiteConnection? connection = null
        )
        {
            return IsUsedByRelation(Array.Empty<IFluentRelation>(), conditionsManager, connection);
        }

        public bool IsUsedByRelation(
            string FluentRelationName,
            Func<SQLiteConditionsManager<T>, SQLiteConditionsManager<T>> conditionsManager,
            SQLiteConnection? connection = null
        ) => IsUsedByRelation(new[] { FluentRelationName }, conditionsManager, connection);

        public bool IsUsedByRelation(
            string[] FluentRelationNames,
            Func<SQLiteConditionsManager<T>, SQLiteConditionsManager<T>> conditionsManager,
            SQLiteConnection? connection = null
        )
        {
            List<IFluentRelation> FluentRelations = new();

            foreach (var FluentRelationName in FluentRelationNames)
                FluentRelations.Add(
                    OutRelations.FirstOrDefault(t => t.RelationName == FluentRelationName)
                        ?? throw new Exception("Relation not found in out relations")
                );

            if (FluentRelations.Count == 0)
                throw new Exception("NoRelationsPassed");

            return IsUsedByRelation(FluentRelations.ToArray(), conditionsManager, connection);
        }

        public bool IsUsedByRelation(
            IFluentRelation[] FluentRelations,
            Func<SQLiteConditionsManager<T>, SQLiteConditionsManager<T>> conditionsManager,
            SQLiteConnection? connection = null
        )
        {
            if (OutRelations.Count == 0)
                return false;

            // Use the provided connection or the default one
            var dbConnection = connection ?? Connection ?? throw new Exception("ConnectionNotPassed");

            // Ensure the connection is open
            var shouldCloseConnection = dbConnection!.State == ConnectionState.Closed;
            if (shouldCloseConnection)
            {
                dbConnection.Open();
            }

            var manager = conditionsManager(new SQLiteConditionsManager<T>());
            List<string> relationQueries = new();
            if (FluentRelations.Length == 0)
            {
                FluentRelations = OutRelations.ToArray();
                foreach (var relation in OutRelations)
                    relationQueries.Add(SQLiteFluentRepository<T>.GetRelationQuery(relation));
            }
            else
            {
                foreach (var relation in FluentRelations)
                {
                    if (!OutRelations.Any(t => t.RelationName == relation.RelationName))
                        throw new Exception("Relation not found in out relations");

                    relationQueries.Add(SQLiteFluentRepository<T>.GetRelationQuery(relation));
                }
            }

            try
            {
                StringBuilder queryBuilder = new(string.Join(" union ", relationQueries));

                var parameters = new List<SQLiteParameter>();

                if (manager.WhereConditions.Count > 0)
                {
                    foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                    {
                        parameters.AddRange(
                            (SQLiteParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
                        );
                    }
                }
                if (parameters.Count == 0)
                {
                    throw new Exception("NoConditionsPassedAndCannotDeterminePK");
                }

                string query = queryBuilder.ToString();
                using var command = new SQLiteCommand(query, dbConnection);
                if (parameters.Count > 0)
                    command.Parameters.AddRange(parameters.ToArray());
                var count = command.ExecuteScalar();
                return count != null;
            }
            finally
            {
                if (shouldCloseConnection)
                    dbConnection.Close();
            }
        }

        public SQLiteFluentRepository<T> BuildDefaultQueries()
        {
            DeleteQuery = new SQLiteDeleteQuery<T>(Connection);
            InsertQuery = new SQLiteInsertQuery<T>(Connection);
            SelectQuery = new SQLiteSelectQuery<T>(Connection);
            UpdateQuery = new SQLiteUpdateQuery<T>(Connection);
            return this;
        }

        /// <summary>
        /// Resets the identity with a new seed value.
        /// </summary>
        /// <param name="newSeed">New seed.</param>
        public SQLiteFluentRepository<T> ResetIdentity(SQLiteConnection? connection = null, int newSeed = 0)
        {
            string cmdText = $"DBCC CHECKIDENT ('[{TablePrefix}{TableName}]', RESEED, {newSeed})";

            SQLiteConnection dbConnection =
                connection ?? Connection ?? throw new Exception("ConnectionNotPassed");

            new SQLiteCommand(cmdText, dbConnection) { CommandType = CommandType.Text }.ExecuteNonQuery();

            return this;
        }

        public SQLiteSelectQuery<T> Select(SQLiteConnection? connection = null) =>
            new(connection ?? Connection) { };

        public SQLiteRawSelectQuery<T> RawSelect(string sqlString, SQLiteConnection? connection = null) =>
            new(connection ?? Connection) { SelectClause = sqlString };

        public SQLiteInsertQuery<T> Insert(SQLiteConnection? connection = null) =>
            new(connection ?? Connection) { };

        public SQLiteUpdateQuery<T> Update(SQLiteConnection? connection = null) =>
            new(connection ?? Connection) { };

        public SQLiteDeleteQuery<T> Delete(SQLiteConnection? connection = null) =>
            new(connection ?? Connection) { };

        public SQLiteAggregateSelectQuery<T, ResultType> Aggregate<ResultType>(
            SQLiteConnection? connection = null
        )
            where ResultType : IFluentModel, new() => new(connection ?? Connection) { };

        IAggregateSelectQuery<T, ResultType> IFluentRepository<T>.Aggregate<ResultType>(
            DbConnection? connection
        ) => Aggregate<ResultType>((SQLiteConnection?)connection);

        IFluentRepository<T> IFluentRepository<T>.Build() => Build();

        IFluentRepository<T> IFluentRepository<T>.BuildDefaultQueries() => BuildDefaultQueries();

        IDeleteQuery<T> IFluentRepository<T>.Delete(DbConnection? connection) =>
            Delete((SQLiteConnection?)connection);

        IInsertQuery<T> IFluentRepository<T>.Insert(DbConnection? connection) =>
            Insert((SQLiteConnection?)connection);

        IRawSelectQuery<T> IFluentRepository<T>.RawSelect(string sqlString, DbConnection? connection) =>
            RawSelect(sqlString, (SQLiteConnection?)connection);

        ISelectQuery<T> IFluentRepository<T>.Select(DbConnection? connection) =>
            Select((SQLiteConnection?)connection);

        IUpdateQuery<T> IFluentRepository<T>.Update(DbConnection? connection) =>
            Update((SQLiteConnection?)connection);

        bool IFluentRepository<T>.IsUsedByAnyOutRelation(
            Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager,
            DbConnection? connection
        )
        {
            throw new NotImplementedException();
        }

        bool IFluentRepository<T>.IsUsedByRelation(
            IFluentRelation[] FluentRelations,
            Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager,
            DbConnection? connection
        ) =>
            IsUsedByRelation(
                FluentRelations,
                (Func<SQLiteConditionsManager<T>, SQLiteConditionsManager<T>>)conditionsManager,
                (SQLiteConnection?)connection
            );

        bool IFluentRepository<T>.IsUsedByRelation(
            string FluentRelationName,
            Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager,
            DbConnection? connection
        ) =>
            IsUsedByRelation(
                FluentRelationName,
                (Func<SQLiteConditionsManager<T>, SQLiteConditionsManager<T>>)conditionsManager,
                (SQLiteConnection?)connection
            );

        bool IFluentRepository<T>.IsUsedByRelation(
            string[] FluentRelationNames,
            Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager,
            DbConnection? connection
        ) =>
            IsUsedByRelation(
                FluentRelationNames,
                (Func<SQLiteConditionsManager<T>, SQLiteConditionsManager<T>>)conditionsManager,
                (SQLiteConnection?)connection
            );

        public SQLiteConnection? Connection { get; set; }

        IDeleteQuery<T>? IFluentRepository<T>.DeleteQuery => DeleteQuery;

        IInsertQuery<T>? IFluentRepository<T>.InsertQuery => InsertQuery;

        ISelectQuery<T>? IFluentRepository<T>.SelectQuery => SelectQuery;

        IUpdateQuery<T>? IFluentRepository<T>.UpdateQuery => UpdateQuery;

        DbConnection? IFluentRepository<T>.Connection => Connection;

        public SQLiteFluentRepository(SQLiteConnection? connection = null)
        {
            Connection = connection;
        }
    }
}