using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace LazurdIT.FluentOrm.Oracle
{
    public class OracleIsBetweenCondition<T, TProperty> : OracleValuesCondition<T, TProperty>, ICondition<T, TProperty>, ISingleAttributeCondition
    {
        public override bool HasParameters => true;
        public TProperty? Value2 { get; set; }

        public bool IsNotBetween { get; set; }

        public override DbParameter[]? GetDbParameters(string expressionSymbol) => GetSqlParameters(expressionSymbol);

        public override OracleParameter[]? GetSqlParameters(string _) => new OracleParameter[] { new($"{ParameterName}_1", Value), new($"{ParameterName}_2", Value2) };

        public override string GetExpression(string expressionSymbol) => $"({AttributeName} {(IsNotBetween ? " not" : "")} between {expressionSymbol}{ParameterName}_1 and {expressionSymbol}{ParameterName}_2)";
    }
}