using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Repositories
{
    internal class OrderItemRepository(ApplicationWriteDbContext context) :
        BaseRepository<OrderItem,OrderItemId>(context), IOrderItemRepository;
}
