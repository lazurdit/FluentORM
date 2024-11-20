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
    public abstract class MsSqlFluentRepository<T> : IFluentRepository<T>
        where T : IFluentModel, new()
    {
        public List<IFluentRelation> InRelations { get; set; } = new();
        public List<IFluentRelation> OutRelations { get; set; } = new();
        public MsSqlInsertQuery<T>? InsertQuery { get; private set; }
        public MsSqlUpdateQuery<T>? UpdateQuery { get; private set; }
        public MsSqlDeleteQuery<T>? DeleteQuery { get; private set; }
        public MsSqlSelectQuery<T>? SelectQuery { get; private set; }

        public string ExpressionSymbol => "@";

        public MsSqlFluentRepository<T> Build()
        {
            BuildDefaultQueries();
            return this;
        }

        public string TableName { get; set; } = MsSqlDtoMapper<T>.GetTableName();

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
            Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>> conditionsManager,
            SqlConnection? connection = null
        )
        {
            return IsUsedByRelation(Array.Empty<IFluentRelation>(), conditionsManager, connection);
        }

        public bool IsUsedByRelation(
            string FluentRelationName,
            Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>> conditionsManager,
            SqlConnection? connection = null
        ) => IsUsedByRelation(new[] { FluentRelationName }, conditionsManager, connection);

        public bool IsUsedByRelation(
            string[] FluentRelationNames,
            Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>> conditionsManager,
            SqlConnection? connection = null
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
            Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>> conditionsManager,
            SqlConnection? connection = null
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

            var manager = conditionsManager(new MsSqlConditionsManager<T>());
            List<string> relationQueries = new();
            if (FluentRelations.Length == 0)
            {
                FluentRelations = OutRelations.ToArray();
                foreach (var relation in OutRelations)
                    relationQueries.Add(MsSqlFluentRepository<T>.GetRelationQuery(relation));
            }
            else
            {
                foreach (var relation in FluentRelations)
                {
                    if (!OutRelations.Any(t => t.RelationName == relation.RelationName))
                        throw new Exception("Relation not found in out relations");

                    relationQueries.Add(MsSqlFluentRepository<T>.GetRelationQuery(relation));
                }
            }

            try
            {
                StringBuilder queryBuilder = new(string.Join(" union ", relationQueries));

                var parameters = new List<SqlParameter>();

                if (manager.WhereConditions.Count > 0)
                {
                    foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                    {
                        parameters.AddRange(
                            condition.SetExpressionSymbol(ExpressionSymbol).GetDbParameters().ToNativeDbParameters<SqlParameter>()!
                        );
                    }
                }
                if (parameters.Count == 0)
                {
                    throw new Exception("NoConditionsPassedAndCannotDeterminePK");
                }

                string query = queryBuilder.ToString();
                using var command = new SqlCommand(query, dbConnection);
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

        public MsSqlFluentRepository<T> BuildDefaultQueries()
        {
            DeleteQuery = new MsSqlDeleteQuery<T>(Connection);
            InsertQuery = new MsSqlInsertQuery<T>(Connection);
            SelectQuery = new MsSqlSelectQuery<T>(Connection);
            UpdateQuery = new MsSqlUpdateQuery<T>(Connection);
            return this;
        }

        /// <summary>
        /// Resets the identity with a new seed value.
        /// </summary>
        /// <param name="newSeed">New seed.</param>
        public MsSqlFluentRepository<T> ResetIdentity(SqlConnection? connection = null, int newSeed = 0)
        {
            string cmdText = $"DBCC CHECKIDENT ('[{TablePrefix}{TableName}]', RESEED, {newSeed})";

            SqlConnection dbConnection =
                connection ?? Connection ?? throw new Exception("ConnectionNotPassed");

            new SqlCommand(cmdText, dbConnection) { CommandType = CommandType.Text }.ExecuteNonQuery();

            return this;
        }

        public MsSqlSelectQuery<T> Select(SqlConnection? connection = null) =>
            new(connection ?? Connection) { };

        public MsSqlRawSelectQuery<T> RawSelect(string sqlString, SqlConnection? connection = null) =>
            new(connection ?? Connection) { SelectClause = sqlString };

        public MsSqlInsertQuery<T> Insert(SqlConnection? connection = null) =>
            new(connection ?? Connection) { };

        public MsSqlUpdateQuery<T> Update(SqlConnection? connection = null) =>
            new(connection ?? Connection) { };

        public MsSqlDeleteQuery<T> Delete(SqlConnection? connection = null) =>
            new(connection ?? Connection) { };

        public MsSqlAggregateSelectQuery<T, ResultType> Aggregate<ResultType>(
            SqlConnection? connection = null
        )
            where ResultType : IFluentModel, new() => new(connection ?? Connection) { };

        IAggregateSelectQuery<T, ResultType> IFluentRepository<T>.Aggregate<ResultType>(
            DbConnection? connection
        ) => Aggregate<ResultType>((SqlConnection?)connection);

        IFluentRepository<T> IFluentRepository<T>.Build() => Build();

        IFluentRepository<T> IFluentRepository<T>.BuildDefaultQueries() => BuildDefaultQueries();

        IDeleteQuery<T> IFluentRepository<T>.Delete(DbConnection? connection) =>
            Delete((SqlConnection?)connection);

        IInsertQuery<T> IFluentRepository<T>.Insert(DbConnection? connection) =>
            Insert((SqlConnection?)connection);

        IRawSelectQuery<T> IFluentRepository<T>.RawSelect(string sqlString, DbConnection? connection) =>
            RawSelect(sqlString, (SqlConnection?)connection);

        ISelectQuery<T> IFluentRepository<T>.Select(DbConnection? connection) =>
            Select((SqlConnection?)connection);

        IUpdateQuery<T> IFluentRepository<T>.Update(DbConnection? connection) =>
            Update((SqlConnection?)connection);

        bool IFluentRepository<T>.IsUsedByAnyOutRelation(
            Func<IFluentConditionsManager<T>, IFluentConditionsManager<T>> conditionsManager,
            DbConnection? connection
        )
        {
            throw new NotImplementedException();
        }

        bool IFluentRepository<T>.IsUsedByRelation(
            IFluentRelation[] FluentRelations,
            Func<IFluentConditionsManager<T>, IFluentConditionsManager<T>> conditionsManager,
            DbConnection? connection
        ) =>
            IsUsedByRelation(
                FluentRelations,
                (Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>>)conditionsManager,
                (SqlConnection?)connection
            );

        bool IFluentRepository<T>.IsUsedByRelation(
            string FluentRelationName,
            Func<IFluentConditionsManager<T>, IFluentConditionsManager<T>> conditionsManager,
            DbConnection? connection
        ) =>
            IsUsedByRelation(
                FluentRelationName,
                (Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>>)conditionsManager,
                (SqlConnection?)connection
            );

        bool IFluentRepository<T>.IsUsedByRelation(
            string[] FluentRelationNames,
            Func<IFluentConditionsManager<T>, IFluentConditionsManager<T>> conditionsManager,
            DbConnection? connection
        ) =>
            IsUsedByRelation(
                FluentRelationNames,
                (Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>>)conditionsManager,
                (SqlConnection?)connection
            );

        public SqlConnection? Connection { get; set; }

        IDeleteQuery<T>? IFluentRepository<T>.DeleteQuery => DeleteQuery;

        IInsertQuery<T>? IFluentRepository<T>.InsertQuery => InsertQuery;

        ISelectQuery<T>? IFluentRepository<T>.SelectQuery => SelectQuery;

        IUpdateQuery<T>? IFluentRepository<T>.UpdateQuery => UpdateQuery;

        DbConnection? IFluentRepository<T>.Connection => Connection;

        public MsSqlFluentRepository(SqlConnection? connection = null)
        {
            Connection = connection;
        }

        public MsSqlFluentRepository<T> WithConnetion(SqlConnection? connection)
        {
            Connection = connection;
            return this;
        }

        IFluentRepository<T> IFluentRepository<T>.WithConnetion(DbConnection? connection) => WithConnetion(connection as SqlConnection);
    }
}