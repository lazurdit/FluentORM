using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.Oracle;

public class OracleIsEqualCondition<T, TProperty> : OracleValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition where T : IFluentModel

{
    public override bool HasParameters => true;

    public override string GetExpression(string expressionSymbol) => $"({AttributeName} = {expressionSymbol}{ParameterName})";
}