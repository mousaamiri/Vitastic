using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Cart.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Repositories
{
    internal class CartRepository(ApplicationWriteDbContext context)
        : BaseRepository<Cart, CartId>(context), ICartRepository
    {
        public async Task<Cart?> GetByUserIdAsync(UserId userId, CancellationToken ct = default)
            =>await ExecuteAsync(() =>
                Context.Carts
                    .Include(c=>c.Items)
                    .FirstOrDefaultAsync(d => d.UserId==userId,ct),ct);

        public async Task<Cart?> GetBySessionIdAsync(string sessionId, CancellationToken ct = default)
            => await ExecuteAsync(() =>
                Context.Carts
                    .Include(c => c.Items)
                    .FirstOrDefaultAsync(c => c.SessionId == sessionId, ct), ct);
    }
}
