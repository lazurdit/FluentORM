namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlIsLikeCondition<T, TProperty> : PgsqlValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;

        public bool IsNotLike { get; set; }

        public override string GetExpression() => $"({AttributeName} {(IsNotLike ? " not " : "")} like {ExpressionSymbol}{ParameterName})";
    }
}