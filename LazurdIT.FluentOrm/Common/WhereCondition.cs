using System;

namespace LazurdIT.FluentOrm.Common
{
    public class WhereCondition<T> : ISingleWhereCondition
    {
        public string? AttributeName { get; set; }
        public WhereConditions Condition { get; set; }
        public T[]? Values { get; set; }

        //check if is valid
        public bool IsValid()
        {
            bool isValid = !string.IsNullOrEmpty(AttributeName);
            isValid = isValid && Enum.IsDefined(typeof(WhereConditions), Condition);
            isValid = isValid && (Condition == WhereConditions.IsEmpty || Condition == WhereConditions.IsNotEmpty || Values?.Length > 0);

            return isValid;
        }

        public string GetExpression()
        {
            string condition = Condition switch
            {
                WhereConditions.Equal => "=",
                WhereConditions.NotEqual => "<>",
                WhereConditions.GreaterThan => ">",
                WhereConditions.GreaterThanOrEqual => ">=",
                WhereConditions.LessThan => "<",
                WhereConditions.LessThanOrEqual => "<=",
                WhereConditions.Like => "LIKE",
                WhereConditions.NotLike => "NOT LIKE",
                WhereConditions.In => "IN",
                WhereConditions.NotIn => "NOT IN",
                WhereConditions.IsEmpty => "IS NULL",
                WhereConditions.IsNotEmpty => "IS NULL",
                WhereConditions.InRange => "BETWEEN",
                WhereConditions.NotInrange => "NOT BETWEEN",
                _ => throw new Exception("Invalid Condition")
            };

            string value = Condition switch
            {
                WhereConditions.IsEmpty => string.Empty,
                WhereConditions.IsNotEmpty => string.Empty,
                WhereConditions.In => $"({string.Join(",", Values!)})",
                WhereConditions.NotIn => $"({string.Join(",", Values!)})",
                WhereConditions.InRange => $"({string.Join(",", Values!)})",
                WhereConditions.NotInrange => $"({string.Join(",", Values!)})",
                WhereConditions.Like => $"'{string.Join(",", Values!)}')",
                _ => Values != null && Values.Length > 0 ? Values[0]!.ToString() : ""
            } ?? "";

            return $"{AttributeName} {condition} {value}";
        }
    }
}