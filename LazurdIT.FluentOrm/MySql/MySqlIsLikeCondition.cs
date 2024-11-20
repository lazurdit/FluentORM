namespace LazurdIT.FluentOrm.MySql
{
    public class MySqlIsLikeCondition<T, TProperty> : MySqlValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;

        public bool IsNotLike { get; set; }

        public override string GetExpression() => $"({AttributeName} {(IsNotLike ? " not " : "")} like {ExpressionSymbol}{ParameterName})";
    }
}