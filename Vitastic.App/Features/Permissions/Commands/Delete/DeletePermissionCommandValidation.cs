using FluentValidation;

namespace Vitastic.App.Features.Permissions.Commands.Delete
{
    public sealed class DeletePermissionCommandValidation: AbstractValidator<DeletePermissionCommand>
    {
        public DeletePermissionCommandValidation()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("شناسه مجوز نباید خالی باشد.")
                .NotEqual(Guid.Empty).WithMessage("شناسه مجوز معتبر نیست.");

        }
    }
}