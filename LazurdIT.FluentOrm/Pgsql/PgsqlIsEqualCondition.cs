using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.Pgsql;

public class PgsqlIsEqualCondition<T, TProperty> : PgsqlValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition where T : IFluentModel

{
    public override bool HasParameters => true;

    public override string GetExpression(string expressionSymbol) => $"({AttributeName} = {expressionSymbol}{ParameterName})";
}