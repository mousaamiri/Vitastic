using FluentValidation;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.UpdateProfile
{
    public sealed class UpdateProfileCommandValidation : AbstractValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidation()
        {
            RuleFor(x => x.UserId)
                .NotEqual(Guid.Empty).WithMessage("شناسه کاربر نمی تواند خالی باشد.");
            When(x => !string.IsNullOrEmpty(x.FirstName), () =>
            {
                RuleFor(x => x.FirstName)
                    .MaximumLength(FirstName.MaxLength).WithMessage($"نام نمی تواند بیشتر از {FirstName.MaxLength}   کاراکتر باشد.")
                    .MinimumLength(FirstName.MinLength).WithMessage($"نام نمی تواند کمتر از {FirstName.MinLength} کاراکتر باشد.");
            });
            When(x => !string.IsNullOrEmpty(x.LastName), () =>
            {
                RuleFor(x => x.LastName)
                    .MaximumLength(LastName.MaxLength)
                    .WithMessage($"نام خانوادگی نمی تواند بیشتر از {LastName.MaxLength} کاراکتر باشد.")
                    .MinimumLength(LastName.MinLength)
                    .WithMessage($"نام خانوادگی نمی تواند کمتر از {LastName.MinLength} کاراکتر باشد.");
            });
            When(x => !string.IsNullOrEmpty(x.PhoneNumber), () =>
            {
                RuleFor(x => x.PhoneNumber)
                    .Matches(@"^09[0-9]{9}$")
                    .WithMessage("فرمت شماره تلفن معتبر نیست.")
                    .MaximumLength(PhoneNumber.MaxLength).WithMessage($"شماره تلفن نمی تواند بیشتر از {PhoneNumber.MaxLength} کاراکتر باشد.")
                    .MinimumLength(PhoneNumber.MinLength).WithMessage($"شماره تلفن نمی تواند کمتر از {PhoneNumber.MinLength} کاراکتر باشد.");
            });
        }
    }
}
