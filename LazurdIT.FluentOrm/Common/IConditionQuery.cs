namespace LazurdIT.FluentOrm.Common
{
    public interface IConditionQuery<T> : IFluentQuery where T : IFluentModel, new()
    {
        IConditionsManager<T> ConditionsManager { get; }
    }
}