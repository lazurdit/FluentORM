using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteIsEqualCondition<T, TProperty> : SQLiteValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition where T : IFluentModel

    {
        public override bool HasParameters => true;

        public override string GetExpression(string expressionSymbol) => $"({AttributeName} = {expressionSymbol}{ParameterName})";
    }
}