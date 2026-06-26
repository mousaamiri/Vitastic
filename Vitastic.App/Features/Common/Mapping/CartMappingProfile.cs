using AutoMapper;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Features.Carts.Dtos;
using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Courses;

namespace Vitastic.App.Features.Common.Mapping;

public class CartMappingProfile : Profile
{
    public CartMappingProfile()
    {
        CreateMap<Cart, CartDto>()
            .ForMember(dest => dest.LastModifiedAt, op =>
                op.MapFrom(src => src.UpdatedAt))
            .ForMember(dest => dest.ItemsCount, op
                => op.MapFrom(src => src.ItemsCount))
            .ForMember(dest => dest.Currency, op
                => op.MapFrom(src => src.ItemsTotal.Currency))
            .ForMember(dest => dest.ItemsTotal, op
                => op.MapFrom(src => src.ItemsTotal.Value))

            ;

        CreateMap<CartItem, CartItemDto>()
            .ForMember(dest => dest.CourseImageName,
                opt
                    => opt.MapFrom<CartItemCourseImageUrlResolver, string>(src => src.CourseImageName));
    }
}
#region Resolver

public class CartItemCourseImageUrlResolver(IFileUrlService fileUrlService)
    :  IMemberValueResolver<CartItem, CartItemDto, string, string>
{
    public string Resolve(CartItem source, CartItemDto destination, string sourceMember, string destMember,
        ResolutionContext context) =>
        fileUrlService.GetFileUrl(nameof(Course), source.Id.Value, FileType.Thumbnail, sourceMember);
}

#endregion
