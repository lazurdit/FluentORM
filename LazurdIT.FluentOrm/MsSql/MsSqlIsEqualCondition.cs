using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlIsEqualCondition<T, TProperty> : MsSqlValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition where T : IFluentModel

    {
        public override bool HasParameters => true;

        public override string GetExpression(string expressionSymbol) => $"({AttributeName} = {expressionSymbol}{ParameterName})";
    }
}