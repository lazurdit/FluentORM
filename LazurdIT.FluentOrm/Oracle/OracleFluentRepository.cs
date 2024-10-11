using System.Data;
using System.Data.Common;
using System.Text;
using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.MsSql;
using Oracle.ManagedDataAccess.Client;

namespace LazurdIT.FluentOrm.Oracle;

public abstract class OracleFluentRepository<T> : IFluentRepository<T>
    where T : IFluentModel, new()
{
    public List<IFluentRelation> InRelations { get; set; } = new();
    public List<IFluentRelation> OutRelations { get; set; } = new();
    public OracleInsertQuery<T>? InsertQuery { get; private set; }
    public OracleUpdateQuery<T>? UpdateQuery { get; private set; }
    public OracleDeleteQuery<T>? DeleteQuery { get; private set; }
    public OracleSelectQuery<T>? SelectQuery { get; private set; }

    public string TableName { get; set; } = OracleDtoMapper<T>.GetTableName();

    public string TablePrefix { get; set; }

    public string TableNameWithPrefix => $"{TablePrefix}{TableName}";

    public string ExpressionSymbol => ":";

    public OracleFluentRepository<T> Build()
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
                $@"{(i > 0 ? " and " : "")} {field.TargetFieldName.FinalPropertyName} = :{field.SourceFieldName.FinalPropertyName}"
            );
        }
        if (relationStringBuilder.Length > 0)
            queryString.Append($" where {relationStringBuilder}");
        return queryString.ToString();
    }

    public bool IsUsedByAnyOutRelation(
        Func<OracleConditionsManager<T>, OracleConditionsManager<T>> conditionsManager,
        OracleConnection? connection = null
    )
    {
        return IsUsedByRelation(Array.Empty<IFluentRelation>(), conditionsManager, connection);
    }

    public bool IsUsedByRelation(
        string FluentRelationName,
        Func<OracleConditionsManager<T>, OracleConditionsManager<T>> conditionsManager,
        OracleConnection? connection = null
    ) => IsUsedByRelation(new[] { FluentRelationName }, conditionsManager, connection);

    public bool IsUsedByRelation(
        string[] FluentRelationNames,
        Func<OracleConditionsManager<T>, OracleConditionsManager<T>> conditionsManager,
        OracleConnection? connection = null
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
        Func<OracleConditionsManager<T>, OracleConditionsManager<T>> conditionsManager,
        OracleConnection? connection = null
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
                    parameters.AddRange(
                        (OracleParameter[]?)condition.GetDbParameters(ExpressionSymbol)!
                    );
                }
            }
            if (parameters.Count == 0)
            {
                throw new Exception("NoConditionsPassedAndCannotDeterminePK");
            }

            string query = queryBuilder.ToString();
            using var command = new OracleCommand(query, dbConnection) { BindByName = true };
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

    public OracleFluentRepository<T> BuildDefaultQueries()
    {
        DeleteQuery = new OracleDeleteQuery<T>(Connection);
        InsertQuery = new OracleInsertQuery<T>(Connection);
        SelectQuery = new OracleSelectQuery<T>(Connection);
        UpdateQuery = new OracleUpdateQuery<T>(Connection);
        return this;
    }

    public OracleSelectQuery<T> Select(OracleConnection? connection = null) =>
        new(connection ?? Connection) { };

    public OracleRawSelectQuery<T> RawSelect(
        string sqlString,
        OracleConnection? connection = null
    ) => new(connection ?? Connection) { SelectClause = sqlString };

    public OracleInsertQuery<T> Insert(OracleConnection? connection = null) =>
        new(connection ?? Connection) { };

    public OracleUpdateQuery<T> Update(OracleConnection? connection = null) =>
        new(connection ?? Connection) { };

    public OracleDeleteQuery<T> Delete(OracleConnection? connection = null) =>
        new(connection ?? Connection) { };

    public OracleAggregateSelectQuery<T, ResultType> Aggregate<ResultType>(
        OracleConnection? connection = null
    )
        where ResultType : IFluentModel, new() => new(connection ?? Connection) { };

    IAggregateSelectQuery<T, ResultType> IFluentRepository<T>.Aggregate<ResultType>(
        DbConnection? connection
    ) => Aggregate<ResultType>((OracleConnection?)connection);

    IFluentRepository<T> IFluentRepository<T>.Build() => Build();

    IFluentRepository<T> IFluentRepository<T>.BuildDefaultQueries() => BuildDefaultQueries();

    IDeleteQuery<T> IFluentRepository<T>.Delete(DbConnection? connection) =>
        Delete((OracleConnection?)connection);

    IInsertQuery<T> IFluentRepository<T>.Insert(DbConnection? connection) =>
        Insert((OracleConnection?)connection);

    IRawSelectQuery<T> IFluentRepository<T>.RawSelect(string sqlString, DbConnection? connection) =>
        RawSelect(sqlString, (OracleConnection?)connection);

    ISelectQuery<T> IFluentRepository<T>.Select(DbConnection? connection) =>
        Select((OracleConnection?)connection);

    IUpdateQuery<T> IFluentRepository<T>.Update(DbConnection? connection) =>
        Update((OracleConnection?)connection);

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
            (Func<OracleConditionsManager<T>, OracleConditionsManager<T>>)conditionsManager,
            (OracleConnection?)connection
        );

    bool IFluentRepository<T>.IsUsedByRelation(
        string FluentRelationName,
        Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager,
        DbConnection? connection
    ) =>
        IsUsedByRelation(
            FluentRelationName,
            (Func<OracleConditionsManager<T>, OracleConditionsManager<T>>)conditionsManager,
            (OracleConnection?)connection
        );

    bool IFluentRepository<T>.IsUsedByRelation(
        string[] FluentRelationNames,
        Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager,
        DbConnection? connection
    ) =>
        IsUsedByRelation(
            FluentRelationNames,
            (Func<OracleConditionsManager<T>, OracleConditionsManager<T>>)conditionsManager,
            (OracleConnection?)connection
        );

    public OracleConnection? Connection { get; set; }

    IDeleteQuery<T>? IFluentRepository<T>.DeleteQuery => DeleteQuery;

    IInsertQuery<T>? IFluentRepository<T>.InsertQuery => InsertQuery;

    ISelectQuery<T>? IFluentRepository<T>.SelectQuery => SelectQuery;

    IUpdateQuery<T>? IFluentRepository<T>.UpdateQuery => UpdateQuery;

    DbConnection? IFluentRepository<T>.Connection => Connection;

    public OracleFluentRepository(OracleConnection? connection = null)
    {
        Connection = connection;
    }
}