using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Cart.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories
{
    public interface ICartRepository : IRepository<Cart, CartId>
    {
        /// <summary>
        /// Get cart with all items by user id. Returns null if no cart exists.
        /// </summary>
        Task<Cart?> GetByUserIdAsync(UserId userId, CancellationToken ct = default);

        Task<Cart?> GetBySessionIdAsync(string sessionId, CancellationToken token=default);
    }
}
