using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Cart.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Repositories
{
    internal class CartItemRepository(ApplicationWriteDbContext context)
        : BaseRepository<CartItem, CartItemId>(context), ICartItemRepository;
}