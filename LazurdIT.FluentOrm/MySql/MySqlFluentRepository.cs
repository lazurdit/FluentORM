using LazurdIT.FluentOrm.Common;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace LazurdIT.FluentOrm.MySql
{
    public abstract class MySqlFluentRepository<T> : IFluentRepository<T>
        where T : IFluentModel, new()
    {
        public List<IFluentRelation> InRelations { get; set; } = new();
        public List<IFluentRelation> OutRelations { get; set; } = new();
        public MySqlInsertQuery<T>? InsertQuery { get; private set; }
        public MySqlUpdateQuery<T>? UpdateQuery { get; private set; }
        public MySqlDeleteQuery<T>? DeleteQuery { get; private set; }
        public MySqlSelectQuery<T>? SelectQuery { get; private set; }

        public string TableName { get; set; } = MySqlDtoMapper<T>.GetTableName();

        public string? TablePrefix { get; set; }

        public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

        public string ExpressionSymbol => "@";

        public MySqlFluentRepository<T> Build()
        {
            BuildDefaultQueries();
            return this;
        }

        private static string GetRelationQuery(IFluentRelation relation)
        {
            StringBuilder queryString = new($@"select top 1 1 FROM {relation.TargetTablePrefix}{relation.TargetTableName}");

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
            Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>> conditionsManager,
            MySqlConnection? connection = null
        )
        {
            return IsUsedByRelation(Array.Empty<IFluentRelation>(), conditionsManager, connection);
        }

        public bool IsUsedByRelation(
            string FluentRelationName,
            Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>> conditionsManager,
            MySqlConnection? connection = null
        ) => IsUsedByRelation(new[] { FluentRelationName }, conditionsManager, connection);

        public bool IsUsedByRelation(
            string[] FluentRelationNames,
            Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>> conditionsManager,
            MySqlConnection? connection = null
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
            Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>> conditionsManager,
            MySqlConnection? connection = null
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

            var manager = conditionsManager(new MySqlConditionsManager<T>());
            List<string> relationQueries = new();
            if (FluentRelations.Length == 0)
            {
                FluentRelations = OutRelations.ToArray();
                foreach (var relation in OutRelations)
                    relationQueries.Add(MySqlFluentRepository<T>.GetRelationQuery(relation));
            }
            else
            {
                foreach (var relation in FluentRelations)
                {
                    if (!OutRelations.Any(t => t.RelationName == relation.RelationName))
                        throw new Exception("Relation not found in out relations");

                    relationQueries.Add(MySqlFluentRepository<T>.GetRelationQuery(relation));
                }
            }

            try
            {
                StringBuilder queryBuilder = new(string.Join(" union ", relationQueries));

                var parameters = new List<MySqlParameter>();

                if (manager.WhereConditions.Count > 0)
                {
                    foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                    {
                        parameters.AddRange(
                            (MySqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
                        );
                    }
                }
                if (parameters.Count == 0)
                {
                    throw new Exception("NoConditionsPassedAndCannotDeterminePK");
                }

                string query = queryBuilder.ToString();
                using var command = new MySqlCommand(query, dbConnection);
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

        public MySqlFluentRepository<T> BuildDefaultQueries()
        {
            DeleteQuery = new MySqlDeleteQuery<T>(Connection);
            InsertQuery = new MySqlInsertQuery<T>(Connection);
            SelectQuery = new MySqlSelectQuery<T>(Connection);
            UpdateQuery = new MySqlUpdateQuery<T>(Connection);
            return this;
        }

        public MySqlSelectQuery<T> Select(MySqlConnection? connection = null) =>
            new(connection ?? Connection) { };

        public MySqlRawSelectQuery<T> RawSelect(string sqlString, MySqlConnection? connection = null) =>
            new(connection ?? Connection) { SelectClause = sqlString };

        public MySqlInsertQuery<T> Insert(MySqlConnection? connection = null) =>
            new(connection ?? Connection) { };

        public MySqlUpdateQuery<T> Update(MySqlConnection? connection = null) =>
            new(connection ?? Connection) { };

        public MySqlDeleteQuery<T> Delete(MySqlConnection? connection = null) =>
            new(connection ?? Connection) { };

        public MySqlAggregateSelectQuery<T, ResultType> Aggregate<ResultType>(
            MySqlConnection? connection = null
        )
            where ResultType : IFluentModel, new() => new(connection ?? Connection) { };

        IAggregateSelectQuery<T, ResultType> IFluentRepository<T>.Aggregate<ResultType>(
            DbConnection? connection
        ) => Aggregate<ResultType>((MySqlConnection?)connection);

        IFluentRepository<T> IFluentRepository<T>.Build() => Build();

        IFluentRepository<T> IFluentRepository<T>.BuildDefaultQueries() => BuildDefaultQueries();

        IDeleteQuery<T> IFluentRepository<T>.Delete(DbConnection? connection) =>
            Delete((MySqlConnection?)connection);

        IInsertQuery<T> IFluentRepository<T>.Insert(DbConnection? connection) =>
            Insert((MySqlConnection?)connection);

        IRawSelectQuery<T> IFluentRepository<T>.RawSelect(string sqlString, DbConnection? connection) =>
            RawSelect(sqlString, (MySqlConnection?)connection);

        ISelectQuery<T> IFluentRepository<T>.Select(DbConnection? connection) =>
            Select((MySqlConnection?)connection);

        IUpdateQuery<T> IFluentRepository<T>.Update(DbConnection? connection) =>
            Update((MySqlConnection?)connection);

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
                (Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>>)conditionsManager,
                (MySqlConnection?)connection
            );

        bool IFluentRepository<T>.IsUsedByRelation(
            string FluentRelationName,
            Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager,
            DbConnection? connection
        ) =>
            IsUsedByRelation(
                FluentRelationName,
                (Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>>)conditionsManager,
                (MySqlConnection?)connection
            );

        bool IFluentRepository<T>.IsUsedByRelation(
            string[] FluentRelationNames,
            Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager,
            DbConnection? connection
        ) =>
            IsUsedByRelation(
                FluentRelationNames,
                (Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>>)conditionsManager,
                (MySqlConnection?)connection
            );

        public MySqlConnection? Connection { get; set; }

        IDeleteQuery<T>? IFluentRepository<T>.DeleteQuery => DeleteQuery;

        IInsertQuery<T>? IFluentRepository<T>.InsertQuery => InsertQuery;

        ISelectQuery<T>? IFluentRepository<T>.SelectQuery => SelectQuery;

        IUpdateQuery<T>? IFluentRepository<T>.UpdateQuery => UpdateQuery;

        DbConnection? IFluentRepository<T>.Connection => Connection;

        public MySqlFluentRepository(MySqlConnection? connection = null)
        {
            Connection = connection;
        }
    }
}