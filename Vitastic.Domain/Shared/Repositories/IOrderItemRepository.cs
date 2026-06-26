using Vitastic.Domain.Entities.Orders;
using Vitastic.Domain.Entities.Orders.ValueObjects;

namespace Vitastic.Domain.Shared.Repositories;

public interface IOrderItemRepository:IRepository<OrderItem,OrderItemId>;
