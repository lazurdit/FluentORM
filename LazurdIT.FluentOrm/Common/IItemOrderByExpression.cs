namespace LazurdIT.FluentOrm.Common;

public interface IItemOrderByExpression
{
    string Expression { get; }
    bool IsRandom { get; }
}