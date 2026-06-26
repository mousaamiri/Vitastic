using FluentValidation;

namespace Vitastic.App.Features.Orders.Queries.CheckOrderNumberExistence
{
    public sealed class CheckOrderNumberExistenceQueryValidation : AbstractValidator<CheckOrderNumberExistenceQuery>
    {
        public CheckOrderNumberExistenceQueryValidation()
        {
            RuleFor(x => x.OrderNumber).NotEmpty().WithMessage("شماره سفارش نمی تواند خالی باشد.");
            RuleFor(x => x.OrderNumber).MaximumLength(100).WithMessage("شماره سفارش باید حداکثر 100 کاراکتر باشد.");
        }
    }
}