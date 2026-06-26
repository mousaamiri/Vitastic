using FluentValidation;

namespace Vitastic.App.Features.Users.Queries.GetById
{
    public sealed class GetUserQueryValidation : AbstractValidator<GetUserQuery>
    {
        public GetUserQueryValidation()
        {
            RuleFor(x => x.UserId).NotEqual(Guid.Empty).WithMessage("شناسه کاربر نمی تواند خالی باشد.");
        }
    }
}