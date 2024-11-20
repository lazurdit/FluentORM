using System;
using System.Collections.Generic;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Common
{
    public interface IFluentRepository
    {
        string ExpressionSymbol { get; }
    }

    public interface IFluentRepository<T> : IFluentRepository
        where T : IFluentModel, new()
    {
        IDeleteQuery<T>? DeleteQuery { get; }
        List<IFluentRelation> InRelations { get; set; }
        IInsertQuery<T>? InsertQuery { get; }
        List<IFluentRelation> OutRelations { get; set; }
        ISelectQuery<T>? SelectQuery { get; }
        DbConnection? Connection { get; }
        string TableName { get; set; }
        string? TablePrefix { get; set; }
        string TableNameWithPrefix { get; }

        IUpdateQuery<T>? UpdateQuery { get; }

        IAggregateSelectQuery<T, ResultType> Aggregate<ResultType>(DbConnection? connection = null)
            where ResultType : IFluentModel, new();

        IFluentRepository<T> WithConnetion(DbConnection? connection);

        IFluentRepository<T> Build();

        IFluentRepository<T> BuildDefaultQueries();

        IDeleteQuery<T> Delete(DbConnection? connection = null);

        IInsertQuery<T> Insert(DbConnection? connection = null);

        bool IsUsedByAnyOutRelation(
            Func<IFluentConditionsManager<T>, IFluentConditionsManager<T>> conditionsManager,
            DbConnection? connection = null
        );

        bool IsUsedByRelation(
            IFluentRelation[] FluentRelations,
            Func<IFluentConditionsManager<T>, IFluentConditionsManager<T>> conditionsManager,
            DbConnection? connection = null
        );

        bool IsUsedByRelation(
            string FluentRelationName,
            Func<IFluentConditionsManager<T>, IFluentConditionsManager<T>> conditionsManager,
            DbConnection? connection = null
        );

        bool IsUsedByRelation(
            string[] FluentRelationNames,
            Func<IFluentConditionsManager<T>, IFluentConditionsManager<T>> conditionsManager,
            DbConnection? connection = null
        );

        IRawSelectQuery<T> RawSelect(string sqlString, DbConnection? connection = null);

        ISelectQuery<T> Select(DbConnection? connection = null);

        IUpdateQuery<T> Update(DbConnection? connection = null);
    }
}