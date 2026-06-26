using AutoMapper;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Features.Instructors.Dtos;
using Vitastic.App.Features.Orders.Dtos;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Orders;

namespace Vitastic.App.Features.Common.Mapping;

/// <summary>
/// AutoMapper Profile for Order Entity Mappings
/// </summary>
public class OrderMappingProfile : Profile
{
    public OrderMappingProfile()
    {
        // ==================== ORDER MAPPINGS ====================

        // Order -> OrderDto
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.PaymentMethod, opt
                => opt.MapFrom(src => src.PaymentMethod.ToString()))
            .ForMember(dest => dest.ItemsCount, opt
                => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.IsPaid, opt
                => opt.MapFrom(src => src.IsPaid()));

        // Order -> OrderDetailDto
        CreateMap<Order, OrderDetailDto>()
            .ForMember(dest => dest.Status, opt
                => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.PaymentMethod, opt
                => opt.MapFrom(src => src.PaymentMethod.ToString()))
            .ForMember(dest => dest.Items, opt
                => opt.MapFrom(src => src.Items));

        // ==================== ORDER ITEM MAPPINGS ====================

        // OrderItem -> OrderItemDto
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.HasDiscount, opt
                => opt.MapFrom(src => src.HasDiscount()))
            .ForMember(dest => dest.DiscountPercentage, opt
                => opt.MapFrom(src => src.GetDiscountPercentage()))
            .ForMember(src =>src.ThumbnailUrl,op
            => op.MapFrom<OrderItemDtoThumbnailUrlResolver>());
    }
}
#region Resolver

public class OrderItemDtoThumbnailUrlResolver(IFileUrlService fileUrlService)
    :  IValueResolver<OrderItem, OrderItemDto, string>
{

    public string Resolve(OrderItem source, OrderItemDto destination, string destMember, ResolutionContext context)
     => fileUrlService.GetFileUrl(nameof(Course), source.Id.Value, FileType.Thumbnail, source.CourseImageName);
}
#endregion
