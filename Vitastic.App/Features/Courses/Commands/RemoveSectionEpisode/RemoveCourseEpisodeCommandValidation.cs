using FluentValidation;

namespace Vitastic.App.Features.Courses.Commands.RemoveSectionEpisode
{
    public sealed class RemoveCourseEpisodeCommandValidation : AbstractValidator<RemoveCourseEpisodeCommand>
    {
        public RemoveCourseEpisodeCommandValidation()
        {
            RuleFor(x => x.CourseId).NotEqual(Guid.Empty).WithMessage("شناسه دوره نمی تواند خالی باشد.")
                .NotEmpty().WithMessage("شناسه دوره نمی تواند خالی باشد.");

            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("شناسه بخش نمی تواند خالی باشد.");

            RuleFor(x => x.EpisodeId)
                .NotEmpty().WithMessage("شناسه قسمت نمی تواند خالی باشد.");
        }
    }
}