namespace LazurdIT.FluentOrm.Common;

public class FluentTypeDictionary : Dictionary<string, FluentTypeInfo>
{
    public List<string> GetFinalPropertyNames() => this.Select(s => s.Value.FinalPropertyName).ToList();

    public FluentTypeDictionary() : base()
    {
    }

    public FluentTypeDictionary(IDictionary<string, FluentTypeInfo> dictionary) : base(dictionary)
    {
    }

    public FluentTypeDictionary(IDictionary<string, FluentTypeInfo> dictionary, IEqualityComparer<string>? comparer) : base(dictionary, comparer)
    {
    }

    public FluentTypeDictionary(IEnumerable<KeyValuePair<string, FluentTypeInfo>> collection) : base(collection)
    {
    }

    public FluentTypeDictionary(IEnumerable<KeyValuePair<string, FluentTypeInfo>> collection, IEqualityComparer<string>? comparer) : base(collection, comparer)
    {
    }

    public FluentTypeDictionary(IEqualityComparer<string>? comparer) : base(comparer)
    {
    }

    public FluentTypeDictionary(int capacity) : base(capacity)
    {
    }

    public FluentTypeDictionary(int capacity, IEqualityComparer<string>? comparer) : base(capacity, comparer)
    {
    }
}