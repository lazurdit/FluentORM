namespace LazurdIT.FluentOrm.Common;

public class FluentTypeDictionary<T> : Dictionary<string, FluentTypeInfo<T>>
{
    public List<string> GetFinalPropertyNames() => this.Select(s => s.Value.FinalPropertyName).ToList();

    public FluentTypeDictionary() : base()
    {
    }

    public FluentTypeDictionary(IDictionary<string, FluentTypeInfo<T>> dictionary) : base(dictionary)
    {
    }

    public FluentTypeDictionary(IDictionary<string, FluentTypeInfo<T>> dictionary, IEqualityComparer<string>? comparer) : base(dictionary, comparer)
    {
    }

    public FluentTypeDictionary(IEnumerable<KeyValuePair<string, FluentTypeInfo<T>>> collection) : base(collection)
    {
    }

    public FluentTypeDictionary(IEnumerable<KeyValuePair<string, FluentTypeInfo<T>>> collection, IEqualityComparer<string>? comparer) : base(collection, comparer)
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

    public FluentTypeDictionary<T> Clone()
    {
        return new FluentTypeDictionary<T>(this);
    }
}