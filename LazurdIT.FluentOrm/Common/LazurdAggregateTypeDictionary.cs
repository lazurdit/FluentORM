using LazurdIT.FluentOrm.Common;
using System.Collections.Generic;
using System.Linq;

public class FluentAggregateTypeDictionary : Dictionary<string, FluentAggregateTypeInfo>
{
    public List<string> GetFinalPropertyNames() => this.Select(s => s.Value.FinalPropertyName).ToList();

    public List<string> GetFinalAliases() => this.Select(s => s.Value.FinalAlias).ToList();

    public List<string> GetFinalHeaderStrings() => this.Select(s => s.Value.GetHeaderExpression()).ToList();

    public FluentAggregateTypeDictionary() : base()
    {
    }

    public FluentAggregateTypeDictionary(IDictionary<string, FluentAggregateTypeInfo> dictionary) : base(dictionary)
    {
    }

    public FluentAggregateTypeDictionary(IDictionary<string, FluentAggregateTypeInfo> dictionary, IEqualityComparer<string>? comparer) : base(dictionary, comparer)
    {
    }

    public FluentAggregateTypeDictionary(IEnumerable<KeyValuePair<string, FluentAggregateTypeInfo>> collection) : base(collection)
    {
    }

    public FluentAggregateTypeDictionary(IEnumerable<KeyValuePair<string, FluentAggregateTypeInfo>> collection, IEqualityComparer<string>? comparer) : base(collection, comparer)
    {
    }

    public FluentAggregateTypeDictionary(IEqualityComparer<string>? comparer) : base(comparer)
    {
    }

    public FluentAggregateTypeDictionary(int capacity) : base(capacity)
    {
    }

    public FluentAggregateTypeDictionary(int capacity, IEqualityComparer<string>? comparer) : base(capacity, comparer)
    {
    }
}