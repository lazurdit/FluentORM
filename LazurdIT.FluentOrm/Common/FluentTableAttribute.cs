using System;

namespace LazurdIT.FluentOrm.Common
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class FluentTableAttribute : Attribute
    {
        private readonly string? name;
        private readonly string? schema;

        public FluentTableAttribute(string? name = null, string? schema = null)
        {
            this.name = name;
            this.schema = schema;
        }

        public string? Name => name;
        public string? Schema => schema;
    }
}