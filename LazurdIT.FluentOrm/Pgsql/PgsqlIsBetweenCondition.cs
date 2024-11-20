using Npgsql;

namespace LazurdIT.FluentOrm.Pgsql
{
    public class PgsqlIsBetweenCondition<T, TProperty> : PgsqlValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;
        public TProperty? Value2 { get; set; }

        public bool IsNotBetween { get; set; }

        public override NpgsqlParameter[]? GetSqlParameters() => new NpgsqlParameter[] { new($"{ExpressionSymbol}{ParameterName}_1", Value), new($"{ExpressionSymbol}{ParameterName}_2", Value2) };

        public override string GetExpression() => $"({AttributeName} {(IsNotBetween ? " not" : "")} between {ExpressionSymbol}{ParameterName}_1 and {ExpressionSymbol}{ParameterName}_2)";
    }
}