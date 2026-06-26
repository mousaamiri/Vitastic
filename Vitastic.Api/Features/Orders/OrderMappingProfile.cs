using AutoMapper;
using Vitastic.Api.Features.Orders.Requests;
using Vitastic.Api.Features.Orders.Responses;
using Vitastic.App.Features.Orders.Commands.AddAdminNote;
using Vitastic.App.Features.Orders.Commands.AddCustomerNote;
using Vitastic.App.Features.Orders.Commands.AddItemToOrder;
using Vitastic.App.Features.Orders.Commands.ApplyDiscount;
using Vitastic.App.Features.Orders.Commands.CancelOrder;
using Vitastic.App.Features.Orders.Commands.Create;
using Vitastic.App.Features.Orders.Commands.ProcessPayment;
using Vitastic.App.Features.Orders.Commands.RefundOrder;
using Vitastic.App.Features.Orders.Commands.RemoveItemFromOrder;
using Vitastic.App.Features.Orders.Commands.SetShippingAmount;
using Vitastic.App.Features.Orders.Commands.SetTaxAmount;
using Vitastic.App.Features.Orders.Commands.UpdateContactInformation;
using Vitastic.App.Features.Orders.Dtos;

namespace Vitastic.Api.Features.Orders;

public class OrderMappingProfile:Profile
{
    public OrderMappingProfile()
    {
       CreateMap<AddAdminNoteRequest, AddAdminNoteCommand>();
        CreateMap<AddCustomerNoteRequest, AddCustomerNoteCommand>();
        CreateMap<AddItemToOrderRequest, AddItemToOrderCommand>();
        CreateMap<ApplyDiscountRequest,ApplyDiscountCommand>();
        CreateMap<CancelOrderRequest,CancelOrderCommand>();
        CreateMap<CreateOrderRequest,CreateOrderCommand>();
        CreateMap<ProcessPaymentRequest,ProcessPaymentCommand>();
        CreateMap<RefundOrderRequest,RefundOrderCommand>();
        CreateMap<RemoveItemFromOrderRequest,RemoveItemFromOrderCommand>();
        CreateMap<SetShippingAmountRequest,SetShippingAmountCommand>();
        CreateMap<SetTaxAmountRequest,SetTaxAmountCommand>();
        CreateMap<UpdateContactInformationRequest,UpdateContactInformationCommand>();



        // Dto --> Response
        CreateMap<OrderDetailDto, OrderDetailResponse>();
        CreateMap<OrderDto, OrderResponse>();
        CreateMap<OrderItemDto, OrderItemResponse>();
        CreateMap<OrderStatisticsDto, OrderStatisticsResponse>();
        CreateMap<RevenueStatisticsDto, RevenueStatisticsResponse>();
        CreateMap<UserOrderStatisticsDto, UserOrderStatisticsResponse>();
    }
}
