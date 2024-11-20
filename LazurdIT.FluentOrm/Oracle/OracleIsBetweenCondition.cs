using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Oracle
{
    public class OracleIsBetweenCondition<T, TProperty> : OracleValuesCondition<T, TProperty>
    {
        public override bool HasParameters => true;
        public TProperty? Value2 { get; set; }

        public bool IsNotBetween { get; set; }

        public override DbParameter[]? GetDbParameters() => GetSqlParameters();

        public override OracleParameter[]? GetSqlParameters() => new OracleParameter[] { new($"{ParameterName}_1", Value), new($"{ParameterName}_2", Value2) };

        public override string GetExpression() => $"({AttributeName} {(IsNotBetween ? " not" : "")} between {ExpressionSymbol}{ParameterName}_1 and {ExpressionSymbol}{ParameterName}_2)";
    }
}