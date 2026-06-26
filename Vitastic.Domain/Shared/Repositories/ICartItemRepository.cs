using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Cart.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories
{
    public interface ICartItemRepository : IRepository<CartItem, CartItemId>;
}