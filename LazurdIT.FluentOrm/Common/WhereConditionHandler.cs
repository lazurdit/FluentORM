namespace LazurdIT.FluentOrm.Common;

public static class WhereConditionHandler
{
    public static string BuildWhereClause(this IWhereCondition condition)
    {
        if (condition is WhereConditionGroup group)
            return group.GetExpression();
        else if (condition is ISingleWhereCondition singleCondition && singleCondition.IsValid())
            return singleCondition.GetExpression();

        throw new ArgumentException("Invalid condition provided.");
    }
}