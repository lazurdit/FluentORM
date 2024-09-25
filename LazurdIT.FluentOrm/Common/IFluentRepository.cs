using System.Data.Common;

namespace LazurdIT.FluentOrm.Common;

public interface IFluentRepository
{
    string ExpressionSymbol { get; }
}

public interface IFluentRepository<T> : IFluentRepository where T : IFluentModel, new()
{
    IDeleteQuery<T>? DeleteQuery { get; }
    List<IFluentRelation> InRelations { get; set; }
    IInsertQuery<T>? InsertQuery { get; }
    List<IFluentRelation> OutRelations { get; set; }
    ISelectQuery<T>? SelectQuery { get; }
    DbConnection? Connection { get; }
    string TableName { get; set; }
    IUpdateQuery<T>? UpdateQuery { get; }

    IAggregateSelectQuery<T, ResultType> Aggregate<ResultType>(DbConnection? sqlConnection = null) where ResultType : IFluentModel, new();

    IFluentRepository<T> Build();

    IFluentRepository<T> BuildDefaultQueries();

    IDeleteQuery<T> Delete(DbConnection? sqlConnection = null);

    IInsertQuery<T> Insert(DbConnection? sqlConnection = null);

    bool IsUsedByAnyOutRelation(Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection = null);

    bool IsUsedByRelation(IFluentRelation[] FluentRelations, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection = null);

    bool IsUsedByRelation(string FluentRelationName, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection = null);

    bool IsUsedByRelation(string[] FluentRelationNames, Func<IConditionsManager<T>, IConditionsManager<T>> conditionsManager, DbConnection? sqlConnection = null);

    IRawSelectQuery<T> RawSelect(string sqlString, DbConnection? sqlConnection = null);

    IFluentRepository<T> ResetIdentity(DbConnection? sqlConnection = null, int newSeed = 0);

    ISelectQuery<T> Select(DbConnection? sqlConnection = null);

    IUpdateQuery<T> Update(DbConnection? sqlConnection = null);
}