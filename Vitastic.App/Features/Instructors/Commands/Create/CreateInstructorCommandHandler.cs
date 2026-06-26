using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Instructors.Dtos;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Instructors.Commands.Create;

public sealed class CreateInstructorCommandHandler(
    IUserRepository userRepository,
    IInstructorRepository instructorRepository,
    IMapper mapper)
    : ICommandHandler<CreateInstructorCommand,Guid>
{
    public async Task<Result<Guid>> Handle(CreateInstructorCommand command, CancellationToken cancellationToken)
    {
        Result<UserId> userIdResult = UserId.CreateFrom(command.UserId);
        if (userIdResult.IsFailure)
            return userIdResult.Error;
        var userIsInstructor = await userRepository.UserIsInstructor(userIdResult.Value, cancellationToken);
        if(userIsInstructor)
            return Error.Conflict("CreateInstructorCommand.UserAlreadyIsInstructor", "این کاربر قبلا تبدیل به استاد شده است.");
        User? user = await userRepository.GetByIdAsync(userIdResult.Value, cancellationToken);
        if (user == null)
            return Error.NotFound("CreateInstructorCommand.UserNotFound", "کاربری با این شناسه یافت نشد.");

        Result<InstructorBio> bioResult = InstructorBio.Create(command.Bio);
        if (bioResult.IsFailure)
            return bioResult.Error;

        Result<InstructorExpertise> expertResult = InstructorExpertise.Create(command.Expert);
        if (expertResult.IsFailure)
            return expertResult.Error;

        Result<Instructor> instructorResult = Instructor.Create(user.Id,
            user.UserFullName,user.UserAvatar, bioResult.Value,expertResult.Value);
        if (instructorResult.IsFailure)
            return instructorResult.Error;
        await instructorRepository.AddAsync(instructorResult.Value, cancellationToken);
        return instructorResult.Value.Id.Value;
    }
}
