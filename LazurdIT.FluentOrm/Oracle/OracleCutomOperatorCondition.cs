namespace LazurdIT.FluentOrm.Oracle
{
    public class OracleCutomOperatorCondition<T, TProperty> : OracleValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;

        public string Operator { get; set; } = " = ";

        public override string GetExpression() => $"({AttributeName} {Operator} {ExpressionSymbol}{ParameterName})";
    }
}