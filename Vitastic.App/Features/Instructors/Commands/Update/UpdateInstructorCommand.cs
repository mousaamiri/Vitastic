using Vitastic.App.Common.Abstractions.Files;
using Vitastic.App.Common.Abstractions.Messaging;

namespace Vitastic.App.Features.Instructors.Commands.Update;

public record UpdateInstructorCommand(Guid InstructorId,
    string NewBio
    ):ICommand;
