using AutoMapper;
using Vitastic.Api.Features.Discounts.Requests;
using Vitastic.Api.Features.Discounts.Responses;
using Vitastic.App.Features.Discounts.Commands.Create.Fixed;
using Vitastic.App.Features.Discounts.Commands.Create.Percentage;
using Vitastic.App.Features.Discounts.Commands.SetMaximumAmount;
using Vitastic.App.Features.Discounts.Commands.SetMinimumAmount;
using Vitastic.App.Features.Discounts.Commands.UpdateDiscount;
using Vitastic.App.Features.Discounts.Dtos;

namespace Vitastic.Api.Features.Discounts;

public class DiscountMappingProfile:Profile
{
    public DiscountMappingProfile()
    {
        CreateMap<UpsertDiscountRequest, CreateFixedAmountDiscountCommand>();
        CreateMap<UpsertDiscountRequest, CreatePercentageDiscountCommand>();
        CreateMap<UpsertDiscountRequest, UpdateDiscountCommand>();
        CreateMap<SetMaximumAmountRequest, SetMaximumDiscountAmountCommand>();
        CreateMap<SetMinimumAmountRequest, SetMinimumOrderAmountCommand>();

        CreateMap<DiscountDto, DiscountResponse>();
        CreateMap<DiscountDetailDto, DiscountDetailResponse>();
        CreateMap<DiscountCalculationDto, DiscountCalculationResponse>();
    }
}
