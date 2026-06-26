using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.Infra.Data.Configurations.Base;

public class AggregateRootConfiguration<TEntity,TKey>
    :FullEntityConfiguration<TEntity, TKey>
where TEntity:AggregateRoot<TKey>
where TKey:IEquatable<TKey>
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);
        //----------------------
        // Domain Events - Ignore
        //----------------------
        builder.Ignore(e => e.DomainEvents);
    }
}
