using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Instructors.Commands.AddInstructorRating
{
    public sealed class AddInstructorRatingHandler(
        IInstructorRepository instructorRepository,
        IInstructorRatingRepository ratingRepository)
        : ICommandHandler<AddInstructorRatingCommand>
    {
        public async Task<Result> Handle(AddInstructorRatingCommand request, CancellationToken ct)
        {
            var instructorIdResult = InstructorId.CreateFrom(request.InstructorId);
            if (instructorIdResult.IsFailure)
                return instructorIdResult.Error;

            var instructorIsExisted = await instructorRepository.IsExistAsync(instructorIdResult.Value, ct);
            if (!instructorIsExisted)
                return Error.NotFound("AddInstructorRatingHandler.InstructorNotFound", "مربی مورد نظر یافت نشد.");

            var userIdResult = UserId.CreateFrom(request.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;

            var existingRating = await ratingRepository.FindAsync(instructorIdResult.Value, userIdResult.Value, ct);
            if (existingRating is not null)
                return Error.Conflict("AddInstructorRatingHandler.RelationIsExist", "شما قبلاً به این مربی امتیاز داده‌اید.");

            var ratingResult = InstructorRating.Create(
                instructorIdResult.Value,
                userIdResult.Value,
                request.RatingValue,
                request.Comment);

            if (ratingResult.IsFailure)
                return ratingResult.Error;

            await ratingRepository.AddAsync(ratingResult.Value, ct);

            return Result.Success();
        }
    }
}