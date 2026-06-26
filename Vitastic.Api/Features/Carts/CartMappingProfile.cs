using AutoMapper;
using Vitastic.Api.Features.Carts.Responses;
using Vitastic.App.Features.Carts.Dtos;

namespace Vitastic.Api.Features.Carts;

public class CartMappingProfile: Profile
{
    public CartMappingProfile()
    {
        CreateMap<CartItemDto, CartItemResponse>();
        CreateMap<CartDto, CartResponse>();
    }
}
