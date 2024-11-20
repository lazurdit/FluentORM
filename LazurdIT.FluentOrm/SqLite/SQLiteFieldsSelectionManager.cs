using LazurdIT.FluentOrm.Common;
using LazurdIT.FluentOrm.MySql;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteFieldsSelectionManager<T> : FieldsSelectionManager<T> where T : IFluentModel, new()
    {
        public SQLiteFieldsSelectionManager() : base()
        {
        }

        public override SQLiteFieldsSelectionManager<T> Include<TProperty>(Expression<Func<T, TProperty>> targetProperty)
        {
            if (targetProperty.Body is MemberExpression memberExpression)
            {
                var propertyName = memberExpression.Member.Name;
                if (OriginalFieldsList.TryGetValue(propertyName, out FluentTypeInfo? value))
                    FieldsList[propertyName] = value;
            }

            return this;
        }

        public override IEnumerable<SQLiteParameter> GetSqlParameters(T instance, string parameterName)
        {
            return FieldsList.Select(t => new SQLiteParameter($"@{parameterName}{t.Value.FinalPropertyName}", t.Value.Property.GetValue(instance) ?? DBNull.Value));
        }

        public override SQLiteFieldsSelectionManager<T> ExcludeAll()
        {
            FieldsList = new();
            return this;
        }

        public override SQLiteFieldsSelectionManager<T> Exclude<TProperty>(Expression<Func<T, TProperty>> targetProperty)
        {
            if (targetProperty.Body is MemberExpression memberExpression)
            {
                var propertyName = memberExpression.Member.Name;
                FieldsList.Remove(propertyName);
            }

            return this;
        }

        public override SQLiteFieldsSelectionManager<T> IncludeAll()
        {
            FieldsList = new(TypeCache.GetTypeCache<T>());

            return this;
        }
    }
}