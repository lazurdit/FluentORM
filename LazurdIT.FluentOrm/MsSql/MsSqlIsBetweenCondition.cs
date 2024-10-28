using LazurdIT.FluentOrm.Common;
using System.Data.Common;
using System.Data.SqlClient;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlIsBetweenCondition<T, TProperty> : MsSqlValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
    {
        public override bool HasParameters => true;
        public TProperty? Value2 { get; set; }

        public bool IsNotBetween { get; set; }

        public override DbParameter[]? GetDbParameters(string expressionSymbol) => GetSqlParameters(expressionSymbol);

        public override SqlParameter[]? GetSqlParameters(string expressionSymbol) => new SqlParameter[] { new($"{ParameterName}_1", Value), new($"{ParameterName}_2", Value2) };

        public override string GetExpression(string expressionSymbol) => $"({AttributeName} {(IsNotBetween ? " not" : "")} between {expressionSymbol}{ParameterName}_1 and {expressionSymbol}{ParameterName}_2)";
    }
}