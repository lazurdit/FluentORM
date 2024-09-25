using LazurdIT.FluentOrm.Common;
using Npgsql;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;

namespace LazurdIT.FluentOrm.Pgsql;

public abstract class PgsqlFluentRepository<T> : IFluentRepository<T> where T : IFluentModel, new()
{
    public List<IFluentRelation> InRelations { get; set; } = new();
    public List<IFluentRelation> OutRelations { get; set; } = new();
    public PgsqlInsertQuery<T>? InsertQuery { get; private set; }
    public PgsqlUpdateQuery<T>? UpdateQuery { get; private set; }
    public PgsqlDeleteQuery<T>? DeleteQuery { get; private set; }
    public PgsqlSelectQuery<T>? SelectQuery { get; private set; }
    public string ExpressionSymbol => "@";

    public PgsqlFluentRepository<T> Build()
    {
        BuildDefaultQueries();
        return this;
    }

    private static string GetRelationQuery(IFluentRelation relation)
    {
        StringBuilder queryString = new($@"select top 1 1 FROM {relation.TargetTableName}");

        StringBuilder relationStringBuilder = new();

        for (int i = 0; i < relation.Fields.Count; i++)
        {
            var field = relation.Fields.ElementAt(i);
            relationStringBuilder.Append($@"{(i > 0 ? " and " : "")} {field.TargetFieldName.FinalPropertyName} = @{field.SourceFieldName.FinalPropertyName}");
        }
        if (relationStringBuilder.Length > 0)
            queryString.Append($" where {relationStringBuilder}");
        return queryString.ToString();
    }

    public bool IsUsedByAnyOutRelation(Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>> conditionsManager, NpgsqlConnection? sqlConnection = null)
    {
        return IsUsedByRelation(Array.Empty<IFluentRelation>(), conditionsManager, sqlConnection);
    }

    public bool IsUsedByRelation(string FluentRelationName, Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>> conditionsManager, NpgsqlConnection? sqlConnection = null)
        => IsUsedByRelation(new[] { FluentRelationName }, conditionsManager, sqlConnection);

    public bool IsUsedByRelation(string[] FluentRelationNames, Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>> conditionsManager, NpgsqlConnection? sqlConnection = null)
    {
        List<IFluentRelation> FluentRelations = new();

        foreach (var FluentRelationName in FluentRelationNames)
            FluentRelations.Add(OutRelations.FirstOrDefault(t => t.RelationName == FluentRelationName) ?? throw new Exception("Relation not found in out relations"));

        if (FluentRelations.Count == 0)
            throw new Exception("NoRelationsPassed");

        return IsUsedByRelation(FluentRelations.ToArray(), conditionsManager, sqlConnection);
    }

    public bool IsUsedByRelation(IFluentRelation[] FluentRelations, Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>> conditionsManager, NpgsqlConnection? sqlConnection = null)
    {
        if (OutRelations.Count == 0)
            return false;

        // Use the provided connection or the default one
        var connection = sqlConnection ?? SqlConnection ?? throw new Exception("ConnectionNotPassed");

        // Ensure the connection is open
        var shouldCloseConnection = connection!.State == ConnectionState.Closed;
        if (shouldCloseConnection)
        {
            connection.Open();
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
                    parameters.AddRange((SqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!);
                }
            }
            if (parameters.Count == 0)
            {
                throw new Exception("NoConditionsPassedAndCannotDeterminePK");
            }

            string query = queryBuilder.ToString();
            using var command = new NpgsqlCommand(query, connection);
            if (parameters.Count > 0)
                command.Parameters.AddRange(parameters.ToArray());
            var count = command.ExecuteScalar();
            return count != null;
        }
        finally
        {
            if (shouldCloseConnection)
                connection.Close();
        }
    }

    public PgsqlFluentRepository<T> BuildDefaultQueries()
    {
        DeleteQuery = new PgsqlDeleteQuery<T>(SqlConnection);
        InsertQuery = new PgsqlInsertQuery<T>(SqlConnection);
        SelectQuery = new PgsqlSelectQuery<T>(SqlConnection);
        UpdateQuery = new PgsqlUpdateQuery<T>(SqlConnection);
        return this;
    }

    public string TableName { get; set; } = PgsqlDtoMapper<T>.GetTableName();

    /// <summary>
    /// Resets the identity with a new seed value.
    /// </summary>
    /// <param name="newSeed">New seed.</param>
    public PgsqlFluentRepository<T> ResetIdentity(NpgsqlConnection? sqlConnection = null, int newSeed = 0)
    {
        string cmdText = $"DBCC CHECKIDENT ('[{TableName}]', RESEED, {newSeed})";

        NpgsqlConnection connection = sqlConnection ?? SqlConnection ?? throw new Exception("ConnectionNotPassed");

        new NpgsqlCommand(cmdText, connection) { CommandType = CommandType.Text }.ExecuteNonQuery();

        return this;
    }

    public PgsqlSelectQuery<T> Select(NpgsqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public PgsqlRawSelectQuery<T> RawSelect(string sqlString, NpgsqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection)
    {
        SelectClause = sqlString
    };

    public PgsqlInsertQuery<T> Insert(NpgsqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public PgsqlUpdateQuery<T> Update(NpgsqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public PgsqlDeleteQuery<T> Delete(NpgsqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public PgsqlAggregateSelectQuery<T, ResultType> Aggregate<ResultType>(NpgsqlConnection? sqlConnection = null) where ResultType : IFluentModel, new()
        => new(sqlConnection ?? SqlConnection) { };

    IAggregateSelectQuery<T, ResultType> IFluentRepository<T>.Aggregate<ResultType>(DbConnection? sqlConnection) => Aggregate<ResultType>((NpgsqlConnection?)sqlConnection);

    IFluentRepository<T> IFluentRepository<T>.Build() => Build();

    IFluentRepository<T> IFluentRepository<T>.BuildDefaultQueries() => BuildDefaultQueries();

    IDeleteQuery<T> IFluentRepository<T>.Delete(DbConnection? sqlConnection) => Delete((NpgsqlConnection?)sqlConnection);

    IInsertQuery<T> IFluentRepository<T>.Insert(DbConnection? sqlConnection) => Insert((NpgsqlConnection?)sqlConnection);

    IRawSelectQuery<T> IFluentRepository<T>.RawSelect(string sqlString, DbConnection? sqlConnection) => RawSelect(sqlString, (NpgsqlConnection?)sqlConnection);

    IFluentRepository<T> IFluentRepository<T>.ResetIdentity(DbConnection? sqlConnection, int newSeed) => ResetIdentity((NpgsqlConnection?)sqlConnection, newSeed);

    ISelectQuery<T> IFluentRepository<T>.Select(DbConnection? sqlConnection) => Select((NpgsqlConnection?)sqlConnection);

    IUpdateQuery<T> IFluentRepository<T>.Update(DbConnection? sqlConnection) => Update((NpgsqlConnection?)sqlConnection);

    bool IFluentRepository<T>.IsUsedByAnyOutRelation(Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection)
    {
        throw new NotImplementedException();
    }

    bool IFluentRepository<T>.IsUsedByRelation(IFluentRelation[] FluentRelations, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection) => IsUsedByRelation(FluentRelations, (Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>>)conditionsManager, (NpgsqlConnection?)sqlConnection);

    bool IFluentRepository<T>.IsUsedByRelation(string FluentRelationName, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection) => IsUsedByRelation(FluentRelationName, (Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>>)conditionsManager, (NpgsqlConnection?)sqlConnection);

    bool IFluentRepository<T>.IsUsedByRelation(string[] FluentRelationNames, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection) => IsUsedByRelation(FluentRelationNames, (Func<PgsqlConditionsManager<T>, PgsqlConditionsManager<T>>)conditionsManager, (NpgsqlConnection?)sqlConnection);

    public NpgsqlConnection? SqlConnection { get; set; }

    IDeleteQuery<T>? IFluentRepository<T>.DeleteQuery => DeleteQuery;

    IInsertQuery<T>? IFluentRepository<T>.InsertQuery => InsertQuery;

    ISelectQuery<T>? IFluentRepository<T>.SelectQuery => SelectQuery;

    IUpdateQuery<T>? IFluentRepository<T>.UpdateQuery => UpdateQuery;

    DbConnection? IFluentRepository<T>.Connection => SqlConnection;

    public PgsqlFluentRepository(NpgsqlConnection? sqlConnection = null)
    {
        SqlConnection = sqlConnection;
    }
}