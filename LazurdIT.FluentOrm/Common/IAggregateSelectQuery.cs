﻿using System.Data.Common;

namespace LazurdIT.FluentOrm.Common;

public interface IAggregateSelectQuery<T, ResultType> : IConditionQuery<T> where T : IFluentModel, new() where ResultType : IFluentModel, new()
{
    AggregateFieldsSelectionManager<T> AggregatesManager { get; }
    IFieldsSelectionManager<T> GroupByFieldsManager { get; }
    IHavingConditionsManager<T> HavingConditionsManager { get; }
    AggregateOrderByManager<T> OrderByManager { get; }
    DbConnection? Connection { get; }
    string TableName { get; set; }

    IAggregateSelectQuery<T, ResultType> Aggregate(Action<AggregateFieldsSelectionManager<T>> fn);

    IEnumerable<ResultType> Execute(DbConnection? sqlConnection = null, int pageNumber = 0, int recordsCount = 0);

    IAggregateSelectQuery<T, ResultType> GroupBy(Action<IFieldsSelectionManager<T>> fn);

    IAggregateSelectQuery<T, ResultType> Having(Action<IHavingConditionsManager<T>> fn);

    IAggregateSelectQuery<T, ResultType> OrderBy(Action<AggregateOrderByManager<T>> fn);

    IAggregateSelectQuery<T, ResultType> Where(Action<IConditionsManager<T>> fn);
}