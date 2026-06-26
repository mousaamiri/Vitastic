using FluentValidation;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.App.Features.Instructors.Commands.AddInstructorRating
{
    public class AddInstructorRatingValidator : AbstractValidator<AddInstructorRatingCommand>
    {
        public AddInstructorRatingValidator()
        {
            RuleFor(x => x.RatingValue)
                .InclusiveBetween(Rating.MinValue, Rating.MaxValue);

            RuleFor(x => x.Comment)
                .MaximumLength(1000);
        }
    }
}