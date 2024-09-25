namespace LazurdIT.FluentOrm.Common;

public class RelationFields
{
    public RelationFields(FluentTypeInfo sourceFieldName, FluentTypeInfo targetFieldName)
    {
        SourceFieldName = sourceFieldName ?? throw new ArgumentNullException(nameof(sourceFieldName));
        TargetFieldName = targetFieldName ?? throw new ArgumentNullException(nameof(targetFieldName));
    }

    public FluentTypeInfo SourceFieldName { get; set; }
    public FluentTypeInfo TargetFieldName { get; set; }
}