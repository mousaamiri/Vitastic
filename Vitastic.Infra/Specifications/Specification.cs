using System.Linq.Expressions;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.Infra.Specifications;

public abstract class Specification<TEntity, TKey>(Expression<Func<TEntity, bool>>? criteria)
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public bool IsSplitQuery { get; set; }
    public Expression<Func<TEntity, bool>>? Criteria { get; private set; } = criteria;

    public List<string> IncludeStrings { get; set; }

    public List<Expression<Func<TEntity, object>>> IncludeExpression { get; set; } = [];
    public Expression<Func<TEntity, object>>? OrderByExpression { get; private set; }
    public Expression<Func<TEntity, object>>? OrderByDescendingExpression { get; private set; }

    protected void AddInclude(Expression<Func<TEntity, object>> includesExpression)
        => IncludeExpression.Add(includesExpression);

    protected void AddOrderBy(Expression<Func<TEntity, object>> orderBydExpression)
        => OrderByExpression = orderBydExpression;

    protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescending)
        => OrderByDescendingExpression = orderByDescending;

    protected void AddInclude(string includeString)
        => IncludeStrings.Add(includeString);
}
