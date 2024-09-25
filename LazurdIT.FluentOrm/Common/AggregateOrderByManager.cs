using System.Linq.Expressions;

namespace LazurdIT.FluentOrm.Common;

public class AggregateOrderByManager<T> where T : IFluentModel, new()
{
    public List<IItemOrderByExpression> OrderByColumns { get; }
    private bool hasRandom = false;
    private bool hasNonRandom = false;

    public AggregateOrderByManager()
    {
        OrderByColumns =new();
    }

    public virtual AggregateOrderByManager<T> FromField<TProperty>(Expression<Func<T, TProperty>> property, OrderDirections direction = OrderDirections.Ascending, bool overrideRandom = false)
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

    public AggregateOrderByManager<T> FromCustomAggregate<TProperty>(Expression<Func<T, TProperty>> property, string aggregationMethod, OrderDirections direction = OrderDirections.Ascending, string? alias = null)
    {
        OrderByColumns.Add(new ItemOrderByExpression(Ag.ForField(property, alias, AggregationMethods.Custom, aggregationMethod).FinalAlias, direction));
        return this;
    }

    public virtual AggregateOrderByManager<T> FromSumAggregate<TProperty>(Expression<Func<T, TProperty>> property, OrderDirections direction = OrderDirections.Ascending, string? alias = null)
    {
        OrderByColumns.Add(new ItemOrderByExpression(Ag.SumForField(property, alias).FinalAlias, direction));
        return this;
    }

    public virtual AggregateOrderByManager<T> FromCountAggregate<TProperty>(Expression<Func<T, TProperty>> property, OrderDirections direction = OrderDirections.Ascending, string? alias = null)
    {
        OrderByColumns.Add(new ItemOrderByExpression(Ag.CountForField(property, alias).FinalAlias, direction));
        return this;
    }

    public virtual AggregateOrderByManager<T> FromAvgAggregate<TProperty>(Expression<Func<T, TProperty>> property, OrderDirections direction = OrderDirections.Ascending, string? alias = null)
    {
        OrderByColumns.Add(new ItemOrderByExpression(Ag.AvgForField(property, alias).FinalAlias, direction));
        return this;
    }

    public virtual AggregateOrderByManager<T> FromMinAggregate<TProperty>(Expression<Func<T, TProperty>> property, OrderDirections direction = OrderDirections.Ascending, string? alias = null)
    {
        OrderByColumns.Add(new ItemOrderByExpression(Ag.MinForField(property, alias).FinalAlias, direction));
        return this;
    }

    public virtual AggregateOrderByManager<T> FromMaxAggregate<TProperty>(Expression<Func<T, TProperty>> property, OrderDirections direction = OrderDirections.Ascending, string? alias = null)
    {
        OrderByColumns.Add(new ItemOrderByExpression(Ag.MaxForField(property, alias).FinalAlias, direction));
        return this;
    }

    public virtual AggregateOrderByManager<T> Random(bool resetExisting = false)
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
