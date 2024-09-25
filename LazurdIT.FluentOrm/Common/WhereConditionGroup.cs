using System.Data;

namespace LazurdIT.FluentOrm.Common;

public class WhereConditionGroup : IWhereCondition
{
    public CompareMethods Method { get; set; }
    public List<IWhereCondition> Conditions { get; set; } = new();

    public string GetExpression()
    {
        var conditions = Conditions
            .Where(c => c.IsValid())
            .Select(WhereConditionHandler.BuildWhereClause)
            .Where(clause => !string.IsNullOrEmpty(clause))
            .ToList();

        if (!conditions.Any())
        {
            return string.Empty;
        }

        var separator = Method == CompareMethods.And ? " AND " : " OR ";
        return $"({string.Join(separator, conditions)})";
    }

    public bool IsValid()
    {
        throw new NotImplementedException();
    }
}