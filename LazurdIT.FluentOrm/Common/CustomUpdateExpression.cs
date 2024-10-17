namespace LazurdIT.FluentOrm.Common
{
    public class CustomUpdateExpression : IUpdateExpression
    {
        public string AttributeName { get; }
        private readonly string expression;
        private readonly string replacement;

        public CustomUpdateExpression(string attributeName, string expression, string replacement = "%")
        {
            AttributeName = attributeName;
            this.expression = expression;
            this.replacement = replacement;
        }

        public string GetExpression(string _, string _2)
        {
            return $"{AttributeName} = {expression.Replace(replacement, AttributeName)}";
        }

        public bool HasParameter { get; }
    }
}