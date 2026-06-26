using FluentValidation;

namespace Vitastic.App.Features.Orders.Queries.GetLatestUserOrder
{
    public sealed class GetLatestUserOrderQueryValidation : AbstractValidator<GetLatestUserOrderQuery>
    {
        public GetLatestUserOrderQueryValidation()
        {
            RuleFor(x => x.UserId).NotEqual(Guid.Empty).WithMessage("شناسه کاربر معتبر نمی باشد.");
        }
    }
}