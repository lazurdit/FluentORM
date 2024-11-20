using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace LazurdIT.FluentOrm.Common
{
    public static class DbParameterExtensions
    {
        public static IEnumerable<TParameter>? ToNativeDbParameters<TParameter>(this DbParameter[]? parameters) where TParameter : DbParameter, new()
        {
            if (parameters == null)
                return null;

            return parameters?.Select(x => (TParameter)x);
        }
    }

    //public interface IFluentCondition
    //{
    //    string GetExpression(string expressionSymbol);

    //    bool HasParameters { get; }

    //    DbParameter[]? GetDbParameters(string expressionSymbol);

    //    string ParameterName { get; set; }

    //    ISingleAttributeCondition SetParameterName(string parameterName);
    //}

    //public interface ISingleAttributeCondition : IFluentCondition
    //{
    //    string AttributeName { get; set; }
    //}

    //public interface IFluentCondition<T>
    //{
    //    T? Value { get; set; }
    //}

    //public interface IFluentCondition<T, TProperty>
    //{
    //    TProperty? Value { get; set; }
    //}
}