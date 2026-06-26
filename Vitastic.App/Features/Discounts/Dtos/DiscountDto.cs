namespace Vitastic.App.Features.Discounts.Dtos
{
    public sealed class DiscountDto{

        public  Guid Id{get;private set;}
        public string Code { get; private set; } = null!;
        public string Title { get; private set; } = null!;
        public  string? Type{get;private set;}
        public string Scope { get; private set; } = null!;
        public  decimal Value{get;private set;}
        public string Currency { get; private set; } = null!;
        public  bool IsActive{get;private set;}
        public  bool IsExpired{get;private set;}
        public  DateTimeOffset StartDate{get;private set;}
        public  DateTimeOffset EndDate{get;private set;}
        public  int UsedCount{get;private set;}
        public  int? UsageLimit{get;private set;}
        }
}
