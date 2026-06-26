using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Courses.Commands.AddCourseRating;

public record AddCourseRatingCommand(
    Guid CourseId,
    Guid UserId,
    decimal RatingValue,
    string? Comment
) : ICommand;
