using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Common
{
    public class OrderByManager<T> where T : IFluentModel, new()
    {
        public List<IItemOrderByExpression> OrderByColumns { get; }
        private bool hasRandom = false;
        private bool hasNonRandom = false;

        public OrderByManager()
        {
            OrderByColumns = new();
        }

        public virtual OrderByManager<T> FromField<TProperty>(Expression<Func<T, TProperty>> property, OrderDirections direction = OrderDirections.Ascending, bool overrideRandom = false)
        {
            if (hasRandom)
                if (!overrideRandom)
                    throw new Exception("Random should be the ONLY order by expression");
                else
                    OrderByColumns.Clear();
            hasNonRandom = true;
            OrderByColumns.Add(ItemOrderByExpression.FromField(property, direction));
            return this;
        }

        public virtual OrderByManager<T> Custom<TProperty>(string customOrderByExpression, bool overrideRandom = false)
        {
            if (hasRandom)
                if (!overrideRandom)
                    throw new Exception("Random should be the ONLY order by expression");
                else
                    OrderByColumns.Clear();
            hasNonRandom = true;
            OrderByColumns.Add(new ItemOrderByExpression(customOrderByExpression));
            return this;
        }

        public virtual OrderByManager<T> Random(bool resetExisting = false)
        {
            if (hasRandom)
                return this;

            if (hasNonRandom)
                if (!resetExisting)
                    throw new Exception("Random should be the ONLY order by expression");
                else
                    OrderByColumns.Clear();

            OrderByColumns.Add(new RandomOrderByExpression());
            hasRandom = true;
            return this;
        }
    }
}