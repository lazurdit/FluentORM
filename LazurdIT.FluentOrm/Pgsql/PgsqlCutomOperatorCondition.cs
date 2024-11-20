namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlCutomOperatorCondition<T, TProperty> : PgsqlValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;

        public string Operator { get; set; } = " = ";

        public override string GetExpression() => $"({AttributeName} {Operator} {ExpressionSymbol}{ParameterName})";
    }
}