namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlCutomOperatorCondition<T, TProperty> : MsSqlValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;

        public string Operator { get; set; } = " = ";

        public override string GetExpression() => $"({AttributeName} {Operator} {ExpressionSymbol}{ParameterName})";
    }
}