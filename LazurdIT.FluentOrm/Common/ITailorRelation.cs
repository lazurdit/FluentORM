namespace LazurdIT.FluentOrm.Common;

public interface IFluentRelation
{
    string SourceTableName { get; }
    List<RelationFields> Fields { get; }

    string TargetTableName { get; }

    string RelationName { get; }
}
