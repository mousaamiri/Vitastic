using FluentValidation;

namespace Vitastic.App.Features.Permissions.Queries.GetById
{
    public sealed class GetPermissionByIdQueryValidation : AbstractValidator<GetPermissionByIdQuery>
    {
        public GetPermissionByIdQueryValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("شناسه مجوز نباید خالی باشد.")
                .NotEqual(Guid.Empty).WithMessage("شناسه مجوز معتبر نیست.");
        }
    }
}