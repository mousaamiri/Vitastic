using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Instructors.Commands.DeActive;

public record DeActiveInstructorCommand(Guid InstructorId) : ICommand;
