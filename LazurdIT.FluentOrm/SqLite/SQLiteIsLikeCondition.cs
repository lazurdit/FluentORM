using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteIsLikeCondition<T, TProperty> : SQLiteValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
    {
        public override bool HasParameters => true;

        public bool IsNotLike { get; set; }

        public override string GetExpression(string expressionSymbol) => $"({AttributeName} {(IsNotLike ? " is not " : "")} like {expressionSymbol}{ParameterName})";
    }
}