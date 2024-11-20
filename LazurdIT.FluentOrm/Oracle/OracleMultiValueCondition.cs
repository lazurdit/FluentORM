using LazurdIT.FluentOrm.Common;
using Oracle.ManagedDataAccess.Client;
using System.Data.Common;
using System.Linq;

namespace LazurdIT.FluentOrm.Oracle
{
    public abstract class OracleMultiValueCondition<T, TProperty> : IMultiValueSingleAttributeCondition<T, TProperty>
    {
        public string AttributeName { get; set; } = string.Empty;

        public string ParameterName { get; set; } = string.Empty;

        public TProperty[]? Values { get; set; }

        public OracleParameter[]? GetSqlParameters()
        {
            return Values?.Select((value, index) => new OracleParameter($"{ParameterName}_{index}", value)).ToArray();
        }

        public abstract bool HasParameters { get; }

        public abstract string GetExpression();

        public DbParameter[]? GetDbParameters() => GetSqlParameters();

        public string ExpressionSymbol { get; set; } = string.Empty;

        public OracleMultiValueCondition<T, TProperty> SetParameterName(string parameterName)
        {
            ParameterName = parameterName;
            return this;
        }

        IFluentCondition IFluentCondition.SetParameterName(string parameterName) => SetParameterName(parameterName);

        public OracleMultiValueCondition<T, TProperty> SetExpressionSymbol(string expressionSymbol)
        {
            ExpressionSymbol = expressionSymbol;
            return this;
        }

        IFluentCondition IFluentCondition.SetExpressionSymbol(string parameterName) => SetExpressionSymbol(parameterName);
    }
}