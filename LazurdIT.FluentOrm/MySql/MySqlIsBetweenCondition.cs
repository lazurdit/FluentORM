using LazurdIT.FluentOrm.Common;
using MySqlConnector;
using System.Data.Common;

namespace LazurdIT.FluentOrm.MySql
{
    public class MySqlIsBetweenCondition<T, TProperty> : MySqlValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
    {
        public override bool HasParameters => true;
        public TProperty? Value2 { get; set; }

        public bool IsNotBetween { get; set; }

        public override DbParameter[]? GetDbParameters(string expressionSymbol) => GetSqlParameters(expressionSymbol);

        public override MySqlParameter[]? GetSqlParameters(string expressionSymbol) => new MySqlParameter[] { new($"{expressionSymbol}{ParameterName}_1", Value), new($"{expressionSymbol}{ParameterName}_2", Value2) };

        public override string GetExpression(string expressionSymbol) => $"({AttributeName} {(IsNotBetween ? " not" : "")} between {expressionSymbol}{ParameterName}_1 and {expressionSymbol}{ParameterName}_2)";
    }
}