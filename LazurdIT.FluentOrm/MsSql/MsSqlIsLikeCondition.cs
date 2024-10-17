using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlIsLikeCondition<T, TProperty> : MsSqlValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
    {
        public override bool HasParameters => true;

        public bool IsNotLike { get; set; }

        public override string GetExpression(string expressionSymbol) => $"({AttributeName} {(IsNotLike ? " is not " : "")} like {expressionSymbol}{ParameterName})";
    }
}