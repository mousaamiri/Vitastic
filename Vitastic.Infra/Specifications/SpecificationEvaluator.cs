using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.Infra.Specifications;

public static class SpecificationEvaluator
{
    public static IQueryable<TEntity> GetQuery<TEntity, TKey>(
        IQueryable<TEntity> inputQuery,
        Specification<TEntity, TKey> specification)
        where TEntity : BaseEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        IQueryable<TEntity> query = inputQuery;

        // ─── Apply Criteria ───
        if (specification.Criteria is not null)
        {
            query = query.Where(specification.Criteria);
        }

        // ─── Apply Includes (Expression) ───
        query = specification.IncludeExpression.Aggregate(
            query,
            (current, includeExpression) => current.Include(includeExpression));

        // ─── Apply Includes (String) ───
        query = specification.IncludeStrings.Aggregate(
            query,
            (current, includeString) => current.Include(includeString));

        // ─── Apply OrderBy ───
        if (specification.OrderByExpression is not null)
        {
            query = query.OrderBy(specification.OrderByExpression);
        }

        // ─── Apply OrderByDescending ───
        if (specification.OrderByDescendingExpression is not null)
        {
            query = query.OrderByDescending(specification.OrderByDescendingExpression);
        }

        // ─── Apply Split Query ───
        if (specification.IsSplitQuery)
        {
            query = query.AsSplitQuery();
        }

        return query;
    }
}
