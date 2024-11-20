using System.Data.Common;
using System.Data.SqlClient;

namespace LazurdIT.FluentOrm.MsSql
{
    public class MsSqlIsBetweenCondition<T, TProperty> : MsSqlValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;
        public TProperty? Value2 { get; set; }

        public bool IsNotBetween { get; set; }

        public override DbParameter[]? GetDbParameters() => GetSqlParameters();

        public override SqlParameter[]? GetSqlParameters() => new SqlParameter[] { new($"{ParameterName}_1", Value), new($"{ParameterName}_2", Value2) };

        public override string GetExpression() => $"({AttributeName} {(IsNotBetween ? " not" : "")} between {ExpressionSymbol}{ParameterName}_1 and {ExpressionSymbol}{ParameterName}_2)";
    }
}