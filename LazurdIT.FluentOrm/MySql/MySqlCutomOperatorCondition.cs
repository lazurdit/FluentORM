namespace LazurdIT.FluentOrm.MySql
{
    public class MySqlCutomOperatorCondition<T, TProperty> : MySqlValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;

        public string Operator { get; set; } = " = ";

        public override string GetExpression() => $"({AttributeName} {Operator} {ExpressionSymbol}{ParameterName})";
    }
}