using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Instructors.Commands.DeActive;

public sealed class DeActiveInstructorCommandHandler(
    IInstructorRepository instructorRepository)
    : ICommandHandler<DeActiveInstructorCommand>
{
    public async Task<Result> Handle(DeActiveInstructorCommand request, CancellationToken cancellationToken)
    {
        Result<InstructorId> instructorIdResult =  InstructorId.CreateFrom(request.InstructorId);
        if (instructorIdResult.IsFailure)
            return instructorIdResult.Error;
        Instructor? instructor = await instructorRepository.FindAsync(instructorIdResult.Value, cancellationToken);
        if (instructor is null)
            return Error.NotFound("DeActiveInstructorCommand.InstructorNotFound", "استادی با این شناسه یافت نشد.");
        Result inActiveResult = instructor.Deactivate();
        if (inActiveResult.IsFailure)
            return inActiveResult.Error;
        await instructorRepository.UpdateAsync(instructor,cancellationToken);
        return Result.Success();

    }
}
