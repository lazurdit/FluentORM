using LazurdIT.FluentOrm.Common;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteIsEqualCondition<T, TProperty> : SQLiteValuesCondition<T, TProperty> where T : IFluentModel

    {
        public override bool HasParameters => true;

        public override string GetExpression() => $"({AttributeName} = {ExpressionSymbol}{ParameterName})";
    }
}