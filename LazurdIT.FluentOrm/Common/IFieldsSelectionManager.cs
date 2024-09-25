using System.Data.Common;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Common;

public interface IFieldsSelectionManager<T> where T : IFluentModel, new()
{
    FluentTypeDictionary FieldsList { get; }
    FluentTypeDictionary OriginalFieldsList { get; }
    FluentTypeDictionary IdentityFieldsList { get; }
    FluentTypeDictionary PKFieldsList { get; }

    IFieldsSelectionManager<T> Exclude<TProperty>(Expression<Func<T, TProperty>> targetProperty);

    IFieldsSelectionManager<T> ExcludeAll();

    IEnumerable<DbParameter> GetSqlParameters(T instance, string parameterName);

    IFieldsSelectionManager<T> Include<TProperty>(Expression<Func<T, TProperty>> targetProperty);

    IFieldsSelectionManager<T> IncludeAll();
}