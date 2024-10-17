using System.Collections.Generic;

namespace LazurdIT.FluentOrm.Common
{
    public interface IFluentRelation
    {
        string? SourceTablePrefix { get; }
        string SourceTableName { get; }
        List<RelationFields> Fields { get; }

        string TargetTableName { get; }
        string? TargetTablePrefix { get; }

        string RelationName { get; }
    }
}