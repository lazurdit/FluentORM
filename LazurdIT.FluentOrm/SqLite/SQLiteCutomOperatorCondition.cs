namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteCutomOperatorCondition<T, TProperty> : SQLiteValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;

        public string Operator { get; set; } = " = ";

        public override string GetExpression() => $"({AttributeName} {Operator} {ExpressionSymbol}{ParameterName})";
    }
}