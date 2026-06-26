using FluentValidation;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.App.Features.Courses.Commands.AddCourseRating
{
    public class AddCourseRatingValidator : AbstractValidator<AddCourseRatingCommand>
    {
        public AddCourseRatingValidator()
        {
            RuleFor(x => x.RatingValue)
                .InclusiveBetween(Rating.MinValue, Rating.MaxValue);

            RuleFor(x => x.Comment)
                .MaximumLength(1000);
        }
    }
}