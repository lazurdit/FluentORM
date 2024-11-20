using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.MySql
{
    public class MySqlIsEqualCondition<T, TProperty> : MySqlValuesCondition<T, TProperty> where T : IFluentModel

    {
        public override bool HasParameters => true;

        public override string GetExpression() => $"({AttributeName} = {ExpressionSymbol}{ParameterName})";
    }
}