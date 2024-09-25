using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.MsSql;

public abstract class MsSqlFluentRepository<T> : IFluentRepository<T> where T : IFluentModel, new()
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

    public bool IsUsedByAnyOutRelation(Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>> conditionsManager, SqlConnection? sqlConnection = null)
    {
        return IsUsedByRelation(Array.Empty<IFluentRelation>(), conditionsManager, sqlConnection);
    }

    public bool IsUsedByRelation(string FluentRelationName, Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>> conditionsManager, SqlConnection? sqlConnection = null)
        => IsUsedByRelation(new[] { FluentRelationName }, conditionsManager, sqlConnection);

    public bool IsUsedByRelation(string[] FluentRelationNames, Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>> conditionsManager, SqlConnection? sqlConnection = null)
    {
        List<IFluentRelation> FluentRelations = new();

        foreach (var FluentRelationName in FluentRelationNames)
            FluentRelations.Add(OutRelations.FirstOrDefault(t => t.RelationName == FluentRelationName) ?? throw new Exception("Relation not found in out relations"));

        if (FluentRelations.Count == 0)
            throw new Exception("NoRelationsPassed");

        return IsUsedByRelation(FluentRelations.ToArray(), conditionsManager, sqlConnection);
    }

    public bool IsUsedByRelation(IFluentRelation[] FluentRelations, Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>> conditionsManager, SqlConnection? sqlConnection = null)
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
                    parameters.AddRange((SqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!);
                }
            }
            if (parameters.Count == 0)
            {
                throw new Exception("NoConditionsPassedAndCannotDeterminePK");
            }

            string query = queryBuilder.ToString();
            using var command = new SqlCommand(query, connection);
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

    public MsSqlFluentRepository<T> BuildDefaultQueries()
    {
        DeleteQuery = new MsSqlDeleteQuery<T>(SqlConnection);
        InsertQuery = new MsSqlInsertQuery<T>(SqlConnection);
        SelectQuery = new MsSqlSelectQuery<T>(SqlConnection);
        UpdateQuery = new MsSqlUpdateQuery<T>(SqlConnection);
        return this;
    }

    public string TableName { get; set; } = MsSqlDtoMapper<T>.GetTableName();

    /// <summary>
    /// Resets the identity with a new seed value.
    /// </summary>
    /// <param name="newSeed">New seed.</param>
    public MsSqlFluentRepository<T> ResetIdentity(SqlConnection? sqlConnection = null, int newSeed = 0)
    {
        string cmdText = $"DBCC CHECKIDENT ('[{TableName}]', RESEED, {newSeed})";

        SqlConnection connection = sqlConnection ?? SqlConnection ?? throw new Exception("ConnectionNotPassed");

        new SqlCommand(cmdText, connection) { CommandType = CommandType.Text }.ExecuteNonQuery();

        return this;
    }

    public MsSqlSelectQuery<T> Select(SqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public MsSqlRawSelectQuery<T> RawSelect(string sqlString, SqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection)
    {
        SelectClause = sqlString
    };

    public MsSqlInsertQuery<T> Insert(SqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public MsSqlUpdateQuery<T> Update(SqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public MsSqlDeleteQuery<T> Delete(SqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public MsSqlAggregateSelectQuery<T, ResultType> Aggregate<ResultType>(SqlConnection? sqlConnection = null) where ResultType : IFluentModel, new()
        => new(sqlConnection ?? SqlConnection) { };

    IAggregateSelectQuery<T, ResultType> IFluentRepository<T>.Aggregate<ResultType>(DbConnection? sqlConnection) => Aggregate<ResultType>((SqlConnection?)sqlConnection);

    IFluentRepository<T> IFluentRepository<T>.Build() => Build();

    IFluentRepository<T> IFluentRepository<T>.BuildDefaultQueries() => BuildDefaultQueries();

    IDeleteQuery<T> IFluentRepository<T>.Delete(DbConnection? sqlConnection) => Delete((SqlConnection?)sqlConnection);

    IInsertQuery<T> IFluentRepository<T>.Insert(DbConnection? sqlConnection) => Insert((SqlConnection?)sqlConnection);

    IRawSelectQuery<T> IFluentRepository<T>.RawSelect(string sqlString, DbConnection? sqlConnection) => RawSelect(sqlString, (SqlConnection?)sqlConnection);

    IFluentRepository<T> IFluentRepository<T>.ResetIdentity(DbConnection? sqlConnection, int newSeed) => ResetIdentity((SqlConnection?)sqlConnection, newSeed);

    ISelectQuery<T> IFluentRepository<T>.Select(DbConnection? sqlConnection) => Select((SqlConnection?)sqlConnection);

    IUpdateQuery<T> IFluentRepository<T>.Update(DbConnection? sqlConnection) => Update((SqlConnection?)sqlConnection);

    bool IFluentRepository<T>.IsUsedByAnyOutRelation(Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection)
    {
        throw new NotImplementedException();
    }

    bool IFluentRepository<T>.IsUsedByRelation(IFluentRelation[] FluentRelations, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection) => IsUsedByRelation(FluentRelations, (Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>>)conditionsManager, (SqlConnection?)sqlConnection);

    bool IFluentRepository<T>.IsUsedByRelation(string FluentRelationName, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection) => IsUsedByRelation(FluentRelationName, (Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>>)conditionsManager, (SqlConnection?)sqlConnection);

    bool IFluentRepository<T>.IsUsedByRelation(string[] FluentRelationNames, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection) => IsUsedByRelation(FluentRelationNames, (Func<MsSqlConditionsManager<T>, MsSqlConditionsManager<T>>)conditionsManager, (SqlConnection?)sqlConnection);

    public SqlConnection? SqlConnection { get; set; }

    IDeleteQuery<T>? IFluentRepository<T>.DeleteQuery => DeleteQuery;

    IInsertQuery<T>? IFluentRepository<T>.InsertQuery => InsertQuery;

    ISelectQuery<T>? IFluentRepository<T>.SelectQuery => SelectQuery;

    IUpdateQuery<T>? IFluentRepository<T>.UpdateQuery => UpdateQuery;

    DbConnection? IFluentRepository<T>.Connection => SqlConnection;

    public MsSqlFluentRepository(SqlConnection? sqlConnection = null)
    {
        SqlConnection = sqlConnection;
    }
}