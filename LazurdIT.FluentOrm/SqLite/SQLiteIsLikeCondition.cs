namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteIsLikeCondition<T, TProperty> : SQLiteValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;

        public bool IsNotLike { get; set; }

        public override string GetExpression() => $"({AttributeName} {(IsNotLike ? " not " : "")} like {ExpressionSymbol}{ParameterName})";
    }
}