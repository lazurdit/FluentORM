using System.Data;
using System.Data.Common;
using System.Text;
using LazurdIT.FluentOrm.Common;
using MySqlConnector;

namespace LazurdIT.FluentOrm.MySql;

public abstract class MySqlFluentRepository<T> : IFluentRepository<T> where T : IFluentModel, new()
{
    public List<IFluentRelation> InRelations { get; set; } = new();
    public List<IFluentRelation> OutRelations { get; set; } = new();
    public MySqlInsertQuery<T>? InsertQuery { get; private set; }
    public MySqlUpdateQuery<T>? UpdateQuery { get; private set; }
    public MySqlDeleteQuery<T>? DeleteQuery { get; private set; }
    public MySqlSelectQuery<T>? SelectQuery { get; private set; }

    public string ExpressionSymbol => "@";

    public MySqlFluentRepository<T> Build()
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

    public bool IsUsedByAnyOutRelation(Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>> conditionsManager, MySqlConnection? sqlConnection = null)
    {
        return IsUsedByRelation(Array.Empty<IFluentRelation>(), conditionsManager, sqlConnection);
    }

    public bool IsUsedByRelation(string FluentRelationName, Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>> conditionsManager, MySqlConnection? sqlConnection = null)
        => IsUsedByRelation(new[] { FluentRelationName }, conditionsManager, sqlConnection);

    public bool IsUsedByRelation(string[] FluentRelationNames, Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>> conditionsManager, MySqlConnection? sqlConnection = null)
    {
        List<IFluentRelation> FluentRelations = new();

        foreach (var FluentRelationName in FluentRelationNames)
            FluentRelations.Add(OutRelations.FirstOrDefault(t => t.RelationName == FluentRelationName) ?? throw new Exception("Relation not found in out relations"));

        if (FluentRelations.Count == 0)
            throw new Exception("NoRelationsPassed");

        return IsUsedByRelation(FluentRelations.ToArray(), conditionsManager, sqlConnection);
    }

    public bool IsUsedByRelation(IFluentRelation[] FluentRelations, Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>> conditionsManager, MySqlConnection? sqlConnection = null)
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
                    parameters.AddRange((MySqlParameter[]?)condition.GetDbParameters(ExpressionSymbol)!);
                }
            }
            if (parameters.Count == 0)
            {
                throw new Exception("NoConditionsPassedAndCannotDeterminePK");
            }

            string query = queryBuilder.ToString();
            using var command = new MySqlCommand(query, connection);
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

    public MySqlFluentRepository<T> BuildDefaultQueries()
    {
        DeleteQuery = new MySqlDeleteQuery<T>(SqlConnection);
        InsertQuery = new MySqlInsertQuery<T>(SqlConnection);
        SelectQuery = new MySqlSelectQuery<T>(SqlConnection);
        UpdateQuery = new MySqlUpdateQuery<T>(SqlConnection);
        return this;
    }

    public string TableName { get; set; } = MySqlDtoMapper<T>.GetTableName();

    /// <summary>
    /// Resets the identity with a new seed value.
    /// </summary>
    /// <param name="newSeed">New seed.</param>
    public MySqlFluentRepository<T> ResetIdentity(MySqlConnection? sqlConnection = null, int newSeed = 0)
    {
        string cmdText = $"DBCC CHECKIDENT ('[{TableName}]', RESEED, {newSeed})";

        MySqlConnection connection = sqlConnection ?? SqlConnection ?? throw new Exception("ConnectionNotPassed");

        new MySqlCommand(cmdText, connection) { CommandType = CommandType.Text }.ExecuteNonQuery();

        return this;
    }

    public MySqlSelectQuery<T> Select(MySqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public MySqlRawSelectQuery<T> RawSelect(string sqlString, MySqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection)
    {
        SelectClause = sqlString
    };

    public MySqlInsertQuery<T> Insert(MySqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public MySqlUpdateQuery<T> Update(MySqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public MySqlDeleteQuery<T> Delete(MySqlConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public MySqlAggregateSelectQuery<T, ResultType> Aggregate<ResultType>(MySqlConnection? sqlConnection = null) where ResultType : IFluentModel, new()
        => new(sqlConnection ?? SqlConnection) { };

    IAggregateSelectQuery<T, ResultType> IFluentRepository<T>.Aggregate<ResultType>(DbConnection? sqlConnection) => Aggregate<ResultType>((MySqlConnection?)sqlConnection);

    IFluentRepository<T> IFluentRepository<T>.Build() => Build();

    IFluentRepository<T> IFluentRepository<T>.BuildDefaultQueries() => BuildDefaultQueries();

    IDeleteQuery<T> IFluentRepository<T>.Delete(DbConnection? sqlConnection) => Delete((MySqlConnection?)sqlConnection);

    IInsertQuery<T> IFluentRepository<T>.Insert(DbConnection? sqlConnection) => Insert((MySqlConnection?)sqlConnection);

    IRawSelectQuery<T> IFluentRepository<T>.RawSelect(string sqlString, DbConnection? sqlConnection) => RawSelect(sqlString, (MySqlConnection?)sqlConnection);

    IFluentRepository<T> IFluentRepository<T>.ResetIdentity(DbConnection? sqlConnection, int newSeed) => ResetIdentity((MySqlConnection?)sqlConnection, newSeed);

    ISelectQuery<T> IFluentRepository<T>.Select(DbConnection? sqlConnection) => Select((MySqlConnection?)sqlConnection);

    IUpdateQuery<T> IFluentRepository<T>.Update(DbConnection? sqlConnection) => Update((MySqlConnection?)sqlConnection);

    bool IFluentRepository<T>.IsUsedByAnyOutRelation(Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection)
    {
        throw new NotImplementedException();
    }

    bool IFluentRepository<T>.IsUsedByRelation(IFluentRelation[] FluentRelations, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection) => IsUsedByRelation(FluentRelations, (Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>>)conditionsManager, (MySqlConnection?)sqlConnection);

    bool IFluentRepository<T>.IsUsedByRelation(string FluentRelationName, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection) => IsUsedByRelation(FluentRelationName, (Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>>)conditionsManager, (MySqlConnection?)sqlConnection);

    bool IFluentRepository<T>.IsUsedByRelation(string[] FluentRelationNames, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection) => IsUsedByRelation(FluentRelationNames, (Func<MySqlConditionsManager<T>, MySqlConditionsManager<T>>)conditionsManager, (MySqlConnection?)sqlConnection);

    public MySqlConnection? SqlConnection { get; set; }

    IDeleteQuery<T>? IFluentRepository<T>.DeleteQuery => DeleteQuery;

    IInsertQuery<T>? IFluentRepository<T>.InsertQuery => InsertQuery;

    ISelectQuery<T>? IFluentRepository<T>.SelectQuery => SelectQuery;

    IUpdateQuery<T>? IFluentRepository<T>.UpdateQuery => UpdateQuery;

    DbConnection? IFluentRepository<T>.Connection => SqlConnection;

    public MySqlFluentRepository(MySqlConnection? sqlConnection = null)
    {
        SqlConnection = sqlConnection;
    }
}