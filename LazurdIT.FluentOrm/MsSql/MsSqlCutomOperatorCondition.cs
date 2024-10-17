using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlCutomOperatorCondition<T, TProperty> : MsSqlValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
    {
        public override bool HasParameters => true;

        public string Operator { get; set; } = " = ";

        public override string GetExpression(string expressionSymbol) => $"({AttributeName} {Operator} {expressionSymbol}{ParameterName})";
    }
}