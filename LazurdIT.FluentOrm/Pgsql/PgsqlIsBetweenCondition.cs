using LazurdIT.FluentOrm.Common;
using Npgsql;

namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlIsBetweenCondition<T, TProperty> : PgsqlValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
    {
        public override bool HasParameters => true;
        public TProperty? Value2 { get; set; }

        public bool IsNotBetween { get; set; }

        public override NpgsqlParameter[]? GetSqlParameters(string expressionSymbol) => new NpgsqlParameter[] { new($"{expressionSymbol}{ParameterName}_1", Value), new($"{expressionSymbol}{ParameterName}_2", Value2) };

        public override string GetExpression(string expressionSymbol) => $"({AttributeName} {(IsNotBetween ? " not" : "")} between {expressionSymbol}{ParameterName}_1 and {expressionSymbol}{ParameterName}_2)";
    }
}