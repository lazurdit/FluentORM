using LazurdIT.FluentOrm.MsSql;
using LazurdIT.FluentOrm.Pgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace LazurdIT.FluentOrm.Common
{
    public static class DataExtensions
    {
        public static DataTable ToDataTable<T>(this PgsqlFieldsSelectionManager<T> fieldsSelectionManager, IEnumerable<T> items) where T : IFluentModel, new()
        => fieldsSelectionManager.ToDataTable(null, items);

        public static DataTable ToDataTable<T>(this PgsqlFieldsSelectionManager<T> fieldsSelectionManager, string? tableName, IEnumerable<T> items) where T : IFluentModel, new()
        {
            var dataTable = new DataTable(tableName ?? typeof(T).Name);

            foreach (var field in fieldsSelectionManager.FieldsList)
            {
                //dataTable.Columns.Add(field.Value.FinalPropertyName);
                dataTable.Columns.Add(field.Value.FinalPropertyName, Nullable.GetUnderlyingType(field.Value.Property.PropertyType) ?? field.Value.Property.PropertyType);
            }

            foreach (var item in items)
            {
                var row = dataTable.NewRow();
                foreach (var field in fieldsSelectionManager.FieldsList)
                {
                    row[field.Value.FinalPropertyName] = field.Value.Property.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public static DataTable ToDataTable<T>(this MsSqlFieldsSelectionManager<T> fieldsSelectionManager, IEnumerable<T> items) where T : IFluentModel, new()
        => fieldsSelectionManager.ToDataTable(null, items);

        public static DataTable ToDataTable<T>(this MsSqlFieldsSelectionManager<T> fieldsSelectionManager, string? tableName, IEnumerable<T> items) where T : IFluentModel, new()
        {
            var dataTable = new DataTable(tableName ?? typeof(T).Name);

            foreach (var field in fieldsSelectionManager.FieldsList)
            {
                //dataTable.Columns.Add(field.Value.FinalPropertyName);
                dataTable.Columns.Add(field.Value.FinalPropertyName, Nullable.GetUnderlyingType(field.Value.Property.PropertyType) ?? field.Value.Property.PropertyType);
            }

            foreach (var item in items)
            {
                var row = dataTable.NewRow();
                foreach (var field in fieldsSelectionManager.FieldsList)
                {
                    row[field.Value.FinalPropertyName] = field.Value.Property.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public static DataTable ToDataTable<T>(this IEnumerable<T> items, string? tableName, string[] propertyNames)
        {
            var dataTable = new DataTable(tableName ?? typeof(T).Name);

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .Where(p => propertyNames.Contains(p.Name))
                                      .ToArray();

            foreach (var property in properties)
            {
                //dataTable.Columns.Add(property.Name);
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            foreach (var item in items)
            {
                var row = dataTable.NewRow();
                foreach (var property in properties)
                {
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}