using LazurdIT.FluentOrm.Common;
using System;
using System.Linq;

namespace LazurdIT.FluentOrm.SQLite
{
    public class SQLiteValueRangeCondition<T, TProperty> : SQLiteMultiValueCondition<T, TProperty>
    {
        public SQLiteValueRangeCondition() : base()
        {
            hasParameters = !TypeExtensions.IsNumeric(typeof(TProperty));
        }

        private readonly bool hasParameters = true;
        public override bool HasParameters => hasParameters;

        public bool IsNotInRange { get; set; }

        public override string GetExpression(string expressionSymbol)
        {
            if (hasParameters)
                return $"({AttributeName} {(IsNotInRange ? " not " : "")} in ({string.Join(",", Values?.Select((value, index) => $"{expressionSymbol}{ParameterName}_{index}")?.ToArray() ?? Array.Empty<string>())}{"))"} ";
            else
                if (Values == null || Values.Length == 0)
                return $"({AttributeName} {(IsNotInRange ? " not " : "")} in (null))";
            else
                return $"({AttributeName} {(IsNotInRange ? " not " : "")} in ({string.Join(",", Values)}))";
        }
    }
}