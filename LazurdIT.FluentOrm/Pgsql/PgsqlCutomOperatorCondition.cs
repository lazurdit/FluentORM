using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlCutomOperatorCondition<T, TProperty> : PgsqlValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
    {
        public override bool HasParameters => true;

        public string Operator { get; set; } = " = ";

        public override string GetExpression(string expressionSymbol) => $"({AttributeName} {Operator} {expressionSymbol}{ParameterName})";
    }
}