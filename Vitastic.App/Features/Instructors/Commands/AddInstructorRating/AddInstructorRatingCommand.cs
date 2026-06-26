using MediatR;
using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Instructors.Commands.AddInstructorRating;

public record AddInstructorRatingCommand(
    Guid InstructorId,
    Guid UserId,
    decimal RatingValue,
    string? Comment
) : ICommand;
