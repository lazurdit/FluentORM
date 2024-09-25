namespace LazurdIT.FluentOrm.Common;

public interface IWhereCondition
{
    bool IsValid();

    string GetExpression();
}