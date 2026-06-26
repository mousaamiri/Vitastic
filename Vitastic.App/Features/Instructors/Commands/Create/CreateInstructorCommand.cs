using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Instructors.Commands.Create;

public record CreateInstructorCommand(Guid UserId, string Bio,string Expert) : ICommand<Guid>;
