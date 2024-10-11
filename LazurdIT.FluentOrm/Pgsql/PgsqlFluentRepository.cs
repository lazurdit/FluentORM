using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.MsSql;
using Npgsql;

namespace LazurdIT.FluentOrm.Pgsql;

public abstract class PgsqlFluentRepository<T> : IFluentRepository<T>
    where T : IFluentModel, new()
{
    public List<IFluentRelation> InRelations { get; set; } = new();
    public List<IFluentRelation> OutRelations { get; set; } = new();
    public PgsqlInsertQuery<T>? InsertQuery { get; private set; }
    public PgsqlUpdateQuery<T>? UpdateQuery { get; private set; }
    public PgsqlDeleteQuery<T>? DeleteQuery { get; private set; }
    public PgsqlSelectQuery<T>? SelectQuery { get; private set; }

    public string TableName { get; set; } = PgsqlDtoMapper<T>.GetTableName();

    public string? TablePrefix { get; set; }

    public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

    public string ExpressionSymbol => "@";

    public PgsqlFluentRepository<T> Build()
    {
        BuildDefaultQueries();
        return this;
    }

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
        Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>> conditionsManager,
        NpgsqlConnection? connection = null
    )
    {
        return IsUsedByRelation(Array.Empty<IFluentRelation>(), conditionsManager, connection);
    }

    public bool IsUsedByRelation(
        string FluentRelationName,
        Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>> conditionsManager,
        NpgsqlConnection? connection = null
    ) => IsUsedByRelation(new[] { FluentRelationName }, conditionsManager, connection);

    public bool IsUsedByRelation(
        string[] FluentRelationNames,
        Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>> conditionsManager,
        NpgsqlConnection? connection = null
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
        Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>> conditionsManager,
        NpgsqlConnection? connection = null
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

        var manager = conditionsManager(new PgsqlConditionsManager<T>());
        List<string> relationQueries = new();
        if (FluentRelations.Length == 0)
        {
            FluentRelations = OutRelations.ToArray();
            foreach (var relation in OutRelations)
                relationQueries.Add(PgsqlFluentRepository<T>.GetRelationQuery(relation));
        }
        else
        {
            foreach (var relation in FluentRelations)
            {
                if (!OutRelations.Any(t => t.RelationName == relation.RelationName))
                    throw new Exception("Relation not found in out relations");

                relationQueries.Add(PgsqlFluentRepository<T>.GetRelationQuery(relation));
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
                        (SqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
                    );
                }
            }
            if (parameters.Count == 0)
            {
                throw new Exception("NoConditionsPassedAndCannotDeterminePK");
            }

            string query = queryBuilder.ToString();
            using var command = new NpgsqlCommand(query, dbConnection);
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

    public PgsqlFluentRepository<T> BuildDefaultQueries()
    {
        DeleteQuery = new PgsqlDeleteQuery<T>(Connection);
        InsertQuery = new PgsqlInsertQuery<T>(Connection);
        SelectQuery = new PgsqlSelectQuery<T>(Connection);
        UpdateQuery = new PgsqlUpdateQuery<T>(Connection);
        return this;
    }

    public PgsqlSelectQuery<T> Select(NpgsqlConnection? connection = null) =>
        new(connection ?? Connection) { };

    public PgsqlRawSelectQuery<T> RawSelect(
        string sqlString,
        NpgsqlConnection? connection = null
    ) => new(connection ?? Connection) { SelectClause = sqlString };

    public PgsqlInsertQuery<T> Insert(NpgsqlConnection? connection = null) =>
        new(connection ?? Connection) { };

    public PgsqlUpdateQuery<T> Update(NpgsqlConnection? connection = null) =>
        new(connection ?? Connection) { };

    public PgsqlDeleteQuery<T> Delete(NpgsqlConnection? connection = null) =>
        new(connection ?? Connection) { };

    public PgsqlAggregateSelectQuery<T, ResultType> Aggregate<ResultType>(
        NpgsqlConnection? connection = null
    )
        where ResultType : IFluentModel, new() => new(connection ?? Connection) { };

    IAggregateSelectQuery<T, ResultType> IFluentRepository<T>.Aggregate<ResultType>(
        DbConnection? connection
    ) => Aggregate<ResultType>((NpgsqlConnection?)connection);

    IFluentRepository<T> IFluentRepository<T>.Build() => Build();

    IFluentRepository<T> IFluentRepository<T>.BuildDefaultQueries() => BuildDefaultQueries();

    IDeleteQuery<T> IFluentRepository<T>.Delete(DbConnection? connection) =>
        Delete((NpgsqlConnection?)connection);

    IInsertQuery<T> IFluentRepository<T>.Insert(DbConnection? connection) =>
        Insert((NpgsqlConnection?)connection);

    IRawSelectQuery<T> IFluentRepository<T>.RawSelect(string sqlString, DbConnection? connection) =>
        RawSelect(sqlString, (NpgsqlConnection?)connection);

    ISelectQuery<T> IFluentRepository<T>.Select(DbConnection? connection) =>
        Select((NpgsqlConnection?)connection);

    IUpdateQuery<T> IFluentRepository<T>.Update(DbConnection? connection) =>
        Update((NpgsqlConnection?)connection);

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
            (Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>>)conditionsManager,
            (NpgsqlConnection?)connection
        );

    bool IFluentRepository<T>.IsUsedByRelation(
        string FluentRelationName,
        Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager,
        DbConnection? connection
    ) =>
        IsUsedByRelation(
            FluentRelationName,
            (Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>>)conditionsManager,
            (NpgsqlConnection?)connection
        );

    bool IFluentRepository<T>.IsUsedByRelation(
        string[] FluentRelationNames,
        Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager,
        DbConnection? connection
    ) =>
        IsUsedByRelation(
            FluentRelationNames,
            (Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>>)conditionsManager,
            (NpgsqlConnection?)connection
        );

    public NpgsqlConnection? Connection { get; set; }

    IDeleteQuery<T>? IFluentRepository<T>.DeleteQuery => DeleteQuery;

    IInsertQuery<T>? IFluentRepository<T>.InsertQuery => InsertQuery;

    ISelectQuery<T>? IFluentRepository<T>.SelectQuery => SelectQuery;

    IUpdateQuery<T>? IFluentRepository<T>.UpdateQuery => UpdateQuery;

    DbConnection? IFluentRepository<T>.Connection => Connection;

    public PgsqlFluentRepository(NpgsqlConnection? connection = null)
    {
        Connection = connection;
    }
}