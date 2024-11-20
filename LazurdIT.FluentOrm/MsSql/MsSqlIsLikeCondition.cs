namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlIsLikeCondition<T, TProperty> : MsSqlValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;

        public bool IsNotLike { get; set; }

        public override string GetExpression() => $"({AttributeName} {(IsNotLike ? " not " : "")} like {ExpressionSymbol}{ParameterName})";
    }
}