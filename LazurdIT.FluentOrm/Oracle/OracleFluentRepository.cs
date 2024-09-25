using System.Data;
using System.Data.Common;
using System.Text;
using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;

namespace LazurdIT.FluentOrm.Oracle;

public abstract class OracleFluentRepository<T> : IFluentRepository<T> where T : IFluentModel, new()
{
    public List<IFluentRelation> InRelations { get; set; } = new();
    public List<IFluentRelation> OutRelations { get; set; } = new();
    public OracleInsertQuery<T>? InsertQuery { get; private set; }
    public OracleUpdateQuery<T>? UpdateQuery { get; private set; }
    public OracleDeleteQuery<T>? DeleteQuery { get; private set; }
    public OracleSelectQuery<T>? SelectQuery { get; private set; }

    public string ExpressionSymbol => ":";

    public OracleFluentRepository<T> Build()
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
            relationStringBuilder.Append($@"{(i > 0 ? " and " : "")} {field.TargetFieldName.FinalPropertyName} = :{field.SourceFieldName.FinalPropertyName}");
        }
        if (relationStringBuilder.Length > 0)
            queryString.Append($" where {relationStringBuilder}");
        return queryString.ToString();
    }

    public bool IsUsedByAnyOutRelation(Func<OracleConditionsManager<T>, OracleConditionsManager<T>> conditionsManager, OracleConnection? sqlConnection = null)
    {
        return IsUsedByRelation(Array.Empty<IFluentRelation>(), conditionsManager, sqlConnection);
    }

    public bool IsUsedByRelation(string FluentRelationName, Func<OracleConditionsManager<T>, OracleConditionsManager<T>> conditionsManager, OracleConnection? sqlConnection = null)
        => IsUsedByRelation(new[] { FluentRelationName }, conditionsManager, sqlConnection);

    public bool IsUsedByRelation(string[] FluentRelationNames, Func<OracleConditionsManager<T>, OracleConditionsManager<T>> conditionsManager, OracleConnection? sqlConnection = null)
    {
        List<IFluentRelation> FluentRelations = new();

        foreach (var FluentRelationName in FluentRelationNames)
            FluentRelations.Add(OutRelations.FirstOrDefault(t => t.RelationName == FluentRelationName) ?? throw new Exception("Relation not found in out relations"));

        if (FluentRelations.Count == 0)
            throw new Exception("NoRelationsPassed");

        return IsUsedByRelation(FluentRelations.ToArray(), conditionsManager, sqlConnection);
    }

    public bool IsUsedByRelation(IFluentRelation[] FluentRelations, Func<OracleConditionsManager<T>, OracleConditionsManager<T>> conditionsManager, OracleConnection? sqlConnection = null)
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

        var manager = conditionsManager(new OracleConditionsManager<T>());
        List<string> relationQueries = new();
        if (FluentRelations.Length == 0)
        {
            FluentRelations = OutRelations.ToArray();
            foreach (var relation in OutRelations)
                relationQueries.Add(OracleFluentRepository<T>.GetRelationQuery(relation));
        }
        else
        {
            foreach (var relation in FluentRelations)
            {
                if (!OutRelations.Any(t => t.RelationName == relation.RelationName))
                    throw new Exception("Relation not found in out relations");

                relationQueries.Add(OracleFluentRepository<T>.GetRelationQuery(relation));
            }
        }

        try
        {
            StringBuilder queryBuilder = new(string.Join(" union ", relationQueries));

            var parameters = new List<OracleParameter>();

            if (manager.WhereConditions.Count > 0)
            {
                foreach (var condition in manager.WhereConditions.Where(w => w.HasParameters))
                {
                    parameters.AddRange((OracleParameter[]?)condition.GetDbParameters(ExpressionSymbol)!);
                }
            }
            if (parameters.Count == 0)
            {
                throw new Exception("NoConditionsPassedAndCannotDeterminePK");
            }

            string query = queryBuilder.ToString();
            using var command = new OracleCommand(query, connection) { BindByName = true };
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

    public OracleFluentRepository<T> BuildDefaultQueries()
    {
        DeleteQuery = new OracleDeleteQuery<T>(SqlConnection);
        InsertQuery = new OracleInsertQuery<T>(SqlConnection);
        SelectQuery = new OracleSelectQuery<T>(SqlConnection);
        UpdateQuery = new OracleUpdateQuery<T>(SqlConnection);
        return this;
    }

    public string TableName { get; set; } = OracleDtoMapper<T>.GetTableName();

    /// <summary>
    /// Resets the identity with a new seed value.
    /// </summary>
    /// <param name="newSeed">New seed.</param>
    public OracleFluentRepository<T> ResetIdentity(OracleConnection? sqlConnection = null, int newSeed = 0)
    {
        string cmdText = $"DBCC CHECKIDENT ('[{TableName}]', RESEED, {newSeed})";

        OracleConnection connection = sqlConnection ?? SqlConnection ?? throw new Exception("ConnectionNotPassed");

        new OracleCommand(cmdText, connection) { CommandType = CommandType.Text, BindByName = true }.ExecuteNonQuery();

        return this;
    }

    public OracleSelectQuery<T> Select(OracleConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public OracleRawSelectQuery<T> RawSelect(string sqlString, OracleConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection)
    {
        SelectClause = sqlString
    };

    public OracleInsertQuery<T> Insert(OracleConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public OracleUpdateQuery<T> Update(OracleConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public OracleDeleteQuery<T> Delete(OracleConnection? sqlConnection = null) => new(sqlConnection ?? SqlConnection) { };

    public OracleAggregateSelectQuery<T, ResultType> Aggregate<ResultType>(OracleConnection? sqlConnection = null) where ResultType : IFluentModel, new()
        => new(sqlConnection ?? SqlConnection) { };

    IAggregateSelectQuery<T, ResultType> IFluentRepository<T>.Aggregate<ResultType>(DbConnection? sqlConnection) => Aggregate<ResultType>((OracleConnection?)sqlConnection);

    IFluentRepository<T> IFluentRepository<T>.Build() => Build();

    IFluentRepository<T> IFluentRepository<T>.BuildDefaultQueries() => BuildDefaultQueries();

    IDeleteQuery<T> IFluentRepository<T>.Delete(DbConnection? sqlConnection) => Delete((OracleConnection?)sqlConnection);

    IInsertQuery<T> IFluentRepository<T>.Insert(DbConnection? sqlConnection) => Insert((OracleConnection?)sqlConnection);

    IRawSelectQuery<T> IFluentRepository<T>.RawSelect(string sqlString, DbConnection? sqlConnection) => RawSelect(sqlString, (OracleConnection?)sqlConnection);

    IFluentRepository<T> IFluentRepository<T>.ResetIdentity(DbConnection? sqlConnection, int newSeed) => ResetIdentity((OracleConnection?)sqlConnection, newSeed);

    ISelectQuery<T> IFluentRepository<T>.Select(DbConnection? sqlConnection) => Select((OracleConnection?)sqlConnection);

    IUpdateQuery<T> IFluentRepository<T>.Update(DbConnection? sqlConnection) => Update((OracleConnection?)sqlConnection);

    bool IFluentRepository<T>.IsUsedByAnyOutRelation(Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection)
    {
        throw new NotImplementedException();
    }

    bool IFluentRepository<T>.IsUsedByRelation(IFluentRelation[] FluentRelations, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection) => IsUsedByRelation(FluentRelations, (Func<OracleConditionsManager<T>, OracleConditionsManager<T>>)conditionsManager, (OracleConnection?)sqlConnection);

    bool IFluentRepository<T>.IsUsedByRelation(string FluentRelationName, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection) => IsUsedByRelation(FluentRelationName, (Func<OracleConditionsManager<T>, OracleConditionsManager<T>>)conditionsManager, (OracleConnection?)sqlConnection);

    bool IFluentRepository<T>.IsUsedByRelation(string[] FluentRelationNames, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection) => IsUsedByRelation(FluentRelationNames, (Func<OracleConditionsManager<T>, OracleConditionsManager<T>>)conditionsManager, (OracleConnection?)sqlConnection);

    public OracleConnection? SqlConnection { get; set; }

    IDeleteQuery<T>? IFluentRepository<T>.DeleteQuery => DeleteQuery;

    IInsertQuery<T>? IFluentRepository<T>.InsertQuery => InsertQuery;

    ISelectQuery<T>? IFluentRepository<T>.SelectQuery => SelectQuery;

    IUpdateQuery<T>? IFluentRepository<T>.UpdateQuery => UpdateQuery;

    DbConnection? IFluentRepository<T>.Connection => SqlConnection;

    public OracleFluentRepository(OracleConnection? sqlConnection = null)
    {
        SqlConnection = sqlConnection;
    }
}