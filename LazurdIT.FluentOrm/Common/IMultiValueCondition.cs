namespace LazurdIT.FluentOrm.Common;

public interface IMultiValueCondition<T, TProperty>
{
    TProperty[]? Values { get; set; }
}