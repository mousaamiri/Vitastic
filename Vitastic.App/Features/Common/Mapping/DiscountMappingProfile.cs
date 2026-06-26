using AutoMapper;
using Vitastic.App.Features.Discounts.Dtos;
using Vitastic.Domain.Entities.Discounts;
using Vitastic.Domain.Entities.Discounts.Enums;

namespace Vitastic.App.Features.Common.Mapping;

/// <summary>
/// Discount Entity to DTO Mapping Profile
/// </summary>
public class DiscountMappingProfile : Profile
{
    public DiscountMappingProfile()
    {
        // Discount → DiscountDto
        CreateMap<Discount, DiscountDto>()
            .ForMember(dest => dest.Value, opt
                => opt.MapFrom(src => src.Type.ToString() == "Percentage"
                    ? src.PercentageValue
                    : src.FixedAmountValue!.Value))
            .ForMember(des => des.Currency, otp =>
                otp.MapFrom(src => src.Type.ToString() == "Percentage"
                    ? "N/A"
                    : src.FixedAmountValue!.Currency.Code))
            .ForMember(des => des.IsExpired, otp =>
                otp.MapFrom(src => src.IsExpired()))
            .ForMember(des=>des.IsExpired,otp=>
                otp.MapFrom(src=>src.IsExpired()));

        // Discount → DiscountDetailDto
        CreateMap<Discount, DiscountDetailDto>()
            .ForMember(dest => dest.Value, opt
                => opt.MapFrom(src => src.Type == DiscountType.Percentage
                    ? src.PercentageValue
                    : src.FixedAmountValue!.Value))
            .ForMember(des => des.Currency, otp =>
                otp.MapFrom(src => src.Type == DiscountType.Percentage
                    ? "N/A"
                    : src.FixedAmountValue!.Currency.Code))
            .ForMember(des => des.IsExpired, otp =>
                otp.MapFrom(src => src.IsExpired()))
            .ForMember(des => des.RemainingUsage, otp =>
                otp.MapFrom(src => src.GetRemainingUsage()))
            .ForMember(des => des.ApplicableCourseIds, otp =>
                otp.MapFrom(src => src.ApplicableCourseIds.Select(c => c.Value).ToList()))
            .ForMember(des => des.ApplicableCategoryIds, otp =>
                otp.MapFrom(src => src.ApplicableCategoryIds.Select(c => c.Value).ToList()))
            .ForMember(des => des.ApplicableInstructorIds, otp =>
                otp.MapFrom(src => src.ApplicableInstructorIds.Select(i => i.Value).ToList()));
    }
}
