using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Discounts.Dtos
{
    public sealed class DiscountDetailDto{
        public Guid Id{get; private set;}
        public string Code{get; private set;}=null!;
        public string Title{get; private set;}=null!;
        public string? Description{get; private set;}
        public string Type{get; private set;}=null!;
        public string Scope{get; private set;}=null!;
        public bool IsExpired { get; private set; }
        public decimal Value{get; private set;}
        public string Currency{get; private set;}=string.Empty;
        public decimal? MinimumOrderAmount{get; private set;}
        public decimal? MaximumDiscountAmount{get; private set;}
        public DateTimeOffset StartDate{get; private set;}
        public DateTimeOffset EndDate{get; private set;}
        public bool IsActive{get; private set;}
        public bool IsSingleUse{get; private set;}
        public int UsedCount{get; private set;}
        public int? UsageLimit{get; private set;}
        public int RemainingUsage{get; private set;}
        public List<Guid> ApplicableCourseIds{get; private set;}=[];
        public List<Guid> ApplicableCategoryIds{get; private set;}=[];
        public List<Guid> ApplicableInstructorIds{get; private set;}=[];
        public DateTimeOffset? CreatedA{get; private set;}
    }
}
