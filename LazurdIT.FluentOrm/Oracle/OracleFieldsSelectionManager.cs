using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.MySql;
using Oracle.ManagedDataAccess.Client;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Oracle;

public class OracleFieldsSelectionManager<T> : FieldsSelectionManager<T> where T : IFluentModel, new()
{
    public OracleFieldsSelectionManager() : base()
    {
    }

    public override OracleFieldsSelectionManager<T> Include<TProperty>(Expression<Func<T, TProperty>> targetProperty)
    {
        if (targetProperty.Body is MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;
            if (OriginalFieldsList.TryGetValue(propertyName, out FluentTypeInfo? value))
                FieldsList[propertyName] = value;
        }

        return this;
    }

    public override IEnumerable<OracleParameter> GetSqlParameters(T instance, string parameterName)
    {
        return FieldsList.Select(t => new OracleParameter($":{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance)));
    }

    public override OracleFieldsSelectionManager<T> ExcludeAll()
    {
        FieldsList = new();
        return this;
    }

    public override OracleFieldsSelectionManager<T> Exclude<TProperty>(Expression<Func<T, TProperty>> targetProperty)
    {
        if (targetProperty.Body is MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;
            FieldsList.Remove(propertyName);
        }

        return this;
    }

    public override OracleFieldsSelectionManager<T> IncludeAll()
    {
        FieldsList = new(TypeCache.GetTypeCache<T>());

        return this;
    }

    //IFieldsSelectionManager<T> IFieldsSelectionManager<T>.Exclude<TProperty>(Expression<Func<T, TProperty>> targetProperty) => Exclude(targetProperty);

    //IFieldsSelectionManager<T> IFieldsSelectionManager<T>.ExcludeAll() => ExcludeAll();

    //IEnumerable<DbParameter> IFieldsSelectionManager<T>.GetSqlParameters(T instance, string parameterName) => GetSqlParameters(instance, parameterName);

    //IFieldsSelectionManager<T> IFieldsSelectionManager<T>.Include<TProperty>(Expression<Func<T, TProperty>> targetProperty) => Include(targetProperty);

    //IFieldsSelectionManager<T> IFieldsSelectionManager<T>.IncludeAll() => IncludeAll();
}