using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlIsEqualCondition<T, TProperty> : PgsqlValuesCondition<T, TProperty> where T : IFluentModel

    {
        public override bool HasParameters => true;

        public override string GetExpression() => $"({AttributeName} = {ExpressionSymbol}{ParameterName})";
    }
}