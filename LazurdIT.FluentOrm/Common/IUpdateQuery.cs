using System;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Common
{
    public interface IUpdateQuery<T> : IConditionQuery<T>, ITableRelatedFluentQuery
        where T : IFluentModel, new()
    {
        FluentUpdateCriteriaManager<T> UpdateManager { get; }

        int Execute(T record, DbConnection? connection = null, bool ignoreEmptyConditions = false);

        int Execute(
            T record,
            Action<IConditionsManager<T>> conditionsFn,
            DbConnection? connection = null,
            bool ignoreEmptyConditions = false
        );

        int Execute(
            T record,
            IConditionsManager<T> manager,
            DbConnection? connection = null,
            bool ignoreEmptyConditions = false
        );

        int Execute(DbConnection? connection = null, bool ignoreEmptyConditions = false);

        int Execute(
            Action<IConditionsManager<T>> conditionsFn,
            DbConnection? connection = null,
            bool ignoreEmptyConditions = false
        );

        int Execute(
            IConditionsManager<T> manager,
            DbConnection? connection = null,
            bool ignoreEmptyConditions = false
        );

        IUpdateQuery<T> WithFields(Action<FluentUpdateCriteriaManager<T>> fn);

        IUpdateQuery<T> Where(Action<IConditionsManager<T>> fn);
    }
}