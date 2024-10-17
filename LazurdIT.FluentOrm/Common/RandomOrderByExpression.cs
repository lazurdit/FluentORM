namespace LazurdIT.FluentOrm.Common
{
    public class RandomOrderByExpression : IItemOrderByExpression
    {
        public string Expression { get; }
        public bool IsRandom => true;
        public string AttributeName { get; }
        public OrderDirections Direction { get; }

        public RandomOrderByExpression()
        {
            AttributeName = "NEWID()";
            Direction = OrderDirections.Ascending;
            Expression = $"{AttributeName}";
        }
    }
}