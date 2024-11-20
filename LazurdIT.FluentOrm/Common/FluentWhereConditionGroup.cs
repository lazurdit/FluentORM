using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace LazurdIT.FluentOrm.Common
{
    public class FluentWhereConditionGroup : IFluentGroupCondition
    {
        public CompareMethods CompareMethod { get; set; }
        public List<IFluentCondition> Conditions { get; set; } = new();

        public void BuildParameters()
        {
            var parameters = Conditions
                       .Where(condition => condition.HasParameters)
                       .Select(condition => condition.GetDbParameters());

            if (Conditions.Count <= 0)
            {
                dbParametersGroup = null;
                return;
            }

            List<DbParameter> dbParameters = new();
            foreach (var parameter in parameters)
                if (parameter != null)
                    dbParameters.AddRange(parameter);

            dbParametersGroup = dbParameters.ToArray();
        }

        public string ExpressionSymbol { get; set; } = string.Empty;

        private bool dbParametersBuilt = false;

        public bool HasParameters
        {
            get
            {
                if (!dbParametersBuilt)
                {
                    BuildParameters();
                    dbParametersBuilt = true;
                }
                return dbParametersGroup?.Length > 0;
            }
        }

        public string ParameterName { get; set; } = string.Empty;

        private DbParameter[]? dbParametersGroup = null;

        public DbParameter[]? GetDbParameters()
        {
            if (!dbParametersBuilt)
            {
                BuildParameters();
                dbParametersBuilt = true;
            }
            return dbParametersGroup;
        }

        public string GetExpression()
        {
            List<string> initialConditions = new();
            for (int i = 0; i < Conditions.Count; i++)
            {
                var condition = Conditions[i];
                initialConditions.Add(condition
                                    .SetParameterName($"{ParameterName}_{i}")
                                    .SetExpressionSymbol(ExpressionSymbol)
                                    .GetExpression());
            }
            var conditions = initialConditions.Where(clause => !string.IsNullOrEmpty(clause))
                .ToList();

            if (conditions.Count == 0)
                return string.Empty;

            return $"({string.Join(CompareMethod == CompareMethods.And ? " AND " : " OR ", conditions)})";
        }

        public FluentWhereConditionGroup SetParameterName(string parameterName)
        {
            ParameterName = parameterName;
            return this;
        }

        IFluentCondition IFluentCondition.SetParameterName(string parameterName) => SetParameterName(parameterName);

        public FluentWhereConditionGroup SetExpressionSymbol(string expressionSymbol)
        {
            ExpressionSymbol = expressionSymbol;
            return this;
        }

        IFluentCondition IFluentCondition.SetExpressionSymbol(string expressionSymbol) => SetExpressionSymbol(expressionSymbol);
    }

    //public interface IFluentCondition
    //{
    //    string GetExpression(string expressionSymbol);

    //    bool HasParameters { get; }

    //    DbParameter[]? GetDbParameters(string expressionSymbol);

    //    string ParameterName { get; set; }

    //    ISingleAttributeCondition SetParameterName(string parameterName);
    //}

    //public interface ISingleAttributeCondition : IFluentCondition
    //{
    //    string AttributeName { get; set; }
    //}

    //public interface IFluentCondition<T>
    //{
    //    T? Value { get; set; }
    //}

    //public interface IFluentCondition<T, TProperty>
    //{
    //    TProperty? Value { get; set; }
    //}
}