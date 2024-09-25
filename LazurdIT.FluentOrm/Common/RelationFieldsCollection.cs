namespace LazurdIT.FluentOrm.Common;

public class RelationFieldsCollection : List<IFluentRelation>
{
    public IFluentRelation this[string relationName] => this.FirstOrDefault(r => r.RelationName.Equals(relationName)) ?? throw new KeyNotFoundException();
}
