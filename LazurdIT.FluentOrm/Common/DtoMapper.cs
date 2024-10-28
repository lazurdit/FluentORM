using System;
using System.Data;
using System.Reflection;

namespace LazurdIT.FluentOrm.Common
{
    internal abstract class DtoMapper<T, TCommand> where T : IFluentModel, new() where TCommand : IDbCommand
    {
        public static string GetTableName()
        {
            var attribute = typeof(T).GetCustomAttribute<FluentTableAttribute>();
            string name = attribute?.Name ?? typeof(T).Name;
            return name;
        }

        protected FluentTypeDictionary typeCache;

        public DtoMapper(FluentTypeDictionary? typesRequired = null)
        {
            typeCache = new(typesRequired ?? TypeCache.GetTypeCache<T>());
        }

        public DtoMapper<T, TCommand> ClearFields()
        {
            typeCache = new();
            return this;
        }

        public DtoMapper<T, TCommand> SetFields(params (string propertyName, FluentTypeInfo value)[] names)
        {
            for (int i = 0; i < names.Length; i++)
                typeCache[names[i].propertyName] = names[i].value;

            return this;
        }

        public T? ToDtoModel(DataRowView dataRowView)
        {
            var instance = new T();

            foreach (var (_, FluentTypeInfo) in typeCache)
            {
                var value = dataRowView[FluentTypeInfo.FinalPropertyName];
                if (value != DBNull.Value)
                    FluentTypeInfo.Property.SetValue(instance, value);
            }

            return instance;
        }

        public abstract T? ToDtoModel(TCommand cmd, string paramPrefix = "");

        public virtual T ToDtoModel(IDataReader reader)
        {
            var instance = new T();
            foreach ((_, FluentTypeInfo FluentTypeInfo) in typeCache)
            {
                var columnName = FluentTypeInfo.FinalPropertyName;

                if (reader[columnName] != DBNull.Value)
                {
                    var value = reader[columnName];
                    var val2 = DbTypeComverter.ReverseGetValue(value, FluentTypeInfo.Property.PropertyType);
                    FluentTypeInfo.Property.SetValue(instance, val2);
                }
            }

            return instance;
        }
    }
}