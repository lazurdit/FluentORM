namespace LazurdIT.FluentOrm.Common
{
    public interface IConditionQuery<T> : IFluentQuery where T : IFluentModel, new()
    {
        IFluentConditionsManager<T> ConditionsManager { get; }
    }
}