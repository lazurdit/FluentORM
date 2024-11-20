namespace LazurdIT.FluentOrm.Common
{
    public interface IFluentSingleAttributeCondition<T> : IFluentSingleAttributeCondition
    {
        T? Value { get; set; }
    }

    public interface IFluentSingleAttributeCondition<T, TProperty> : IFluentSingleAttributeCondition
    {
        TProperty? Value { get; set; }
    }
}