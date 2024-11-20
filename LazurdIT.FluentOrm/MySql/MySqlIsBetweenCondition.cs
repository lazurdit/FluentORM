using MySqlConnector;
using System.Data.Common;

namespace LazurdIT.FluentOrm.MySql
{
    public class MySqlIsBetweenCondition<T, TProperty> : MySqlValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;
        public TProperty? Value2 { get; set; }

        public bool IsNotBetween { get; set; }

        public override DbParameter[]? GetDbParameters() => GetSqlParameters();

        public override MySqlParameter[]? GetSqlParameters() => new MySqlParameter[] { new($"{ExpressionSymbol}{ParameterName}_1", Value), new($"{ExpressionSymbol}{ParameterName}_2", Value2) };

        public override string GetExpression() => $"({AttributeName} {(IsNotBetween ? " not" : "")} between {ExpressionSymbol}{ParameterName}_1 and {ExpressionSymbol}{ParameterName}_2)";
    }
}