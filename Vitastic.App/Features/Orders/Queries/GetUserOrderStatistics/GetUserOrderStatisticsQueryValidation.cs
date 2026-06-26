using FluentValidation;

namespace Vitastic.App.Features.Orders.Queries.GetUserOrderStatistics
{
    public sealed class GetUserOrderStatisticsQueryValidation:AbstractValidator<GetUserOrderStatisticsQuery>
    {
        public GetUserOrderStatisticsQueryValidation()
        {
            RuleFor(x => x.UserId)
                .NotEqual(Guid.Empty).WithMessage("شناسه کاربر نمی‌تواند خالی باشد");
        }
    }
}
