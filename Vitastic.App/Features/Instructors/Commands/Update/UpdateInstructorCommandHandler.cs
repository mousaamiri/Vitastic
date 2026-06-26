using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Instructors.Commands.Update;

public sealed class UpdateInstructorCommandHandler(IInstructorRepository instructorRepository, IFileStorageService storageService) : ICommandHandler<UpdateInstructorCommand>
{
    public async Task<Result> Handle(UpdateInstructorCommand command, CancellationToken cancellationToken)
    {
        Result<InstructorId> instructorIdResult = InstructorId.CreateFrom(command.InstructorId);
        if (instructorIdResult.IsFailure)
            return instructorIdResult.Error;
        Instructor? instructor = await instructorRepository.FindAsync(instructorIdResult.Value, cancellationToken);
        if (instructor == null)
            return Error.NotFound("UpdateInstructorCommand.InstructorNotFound", "هیچ استادی با این شناسه یافت نشد. ");
        if (command.NewBio is null)
        {
            return Result.Success();
        }

        Result<InstructorBio> bioResult = InstructorBio.Create(command.NewBio);
        if (bioResult.IsFailure)
            return bioResult.Error;

        Result updateBioResult = instructor.UpdateBio(bioResult.Value);
        if (updateBioResult.IsFailure)
            return updateBioResult.Error;
        return Result.Success();
    }
}
