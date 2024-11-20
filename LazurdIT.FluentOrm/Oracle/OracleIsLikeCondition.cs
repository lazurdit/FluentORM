namespace LazurdIT.FluentOrm.Oracle
{
    public class OracleIsLikeCondition<T, TProperty> : OracleValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;

        public bool IsNotLike { get; set; }

        public override string GetExpression() => $"({AttributeName} {(IsNotLike ? " not " : "")} like {ExpressionSymbol}{ParameterName})";
    }
}