using FluentValidation;

namespace Vitastic.App.Features.Orders.Queries.GetRevenueStatistics
{
    public sealed class GetRevenueStatisticsQueryValidation:AbstractValidator<GetRevenueStatisticsQuery>
    {
        public GetRevenueStatisticsQueryValidation()
        {
            RuleFor(x => x.FromDate)
                .LessThanOrEqualTo(x => x.ToDate)
                .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
                .WithMessage("تاریخ شروع باید کمتر یا مساوی تاریخ پایان باشد");
        }
    }
}