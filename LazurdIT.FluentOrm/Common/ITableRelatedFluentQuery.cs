namespace LazurdIT.FluentOrm.Common
{
    public interface ITableRelatedFluentQuery : IFluentQuery
    {
        string? TablePrefix { get; set; }

        string TableName { get; set; }

        string TableNameWithPrefix { get; }

        ITableRelatedFluentQuery WithPrefix(string prefix);
    }
}