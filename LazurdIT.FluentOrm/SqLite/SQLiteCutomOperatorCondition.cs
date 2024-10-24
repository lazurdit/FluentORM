using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteCutomOperatorCondition<T, TProperty> : SQLiteValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
    {
        public override bool HasParameters => true;

        public string Operator { get; set; } = " = ";

        public override string GetExpression(string expressionSymbol) => $"({AttributeName} {Operator} {expressionSymbol}{ParameterName})";
    }
}