using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Instructors;

public class InstructorRating:BaseEntity<InstructorRatingId>
{
    public InstructorId InstructorId { get; private set; }
    public UserId UserId { get; private set; }
    public Rating Rating { get; private set; }

    public Instructor Instructor { get; set; }
    public User User { get; set; }

    private InstructorRating(){ }

    private InstructorRating(InstructorRatingId id,InstructorId instructorId, UserId userId,
        Rating rating):base(id)
    {
        InstructorId = instructorId;
        UserId = userId;
        Rating = rating;
    }
    public static Result<InstructorRating> Create(
        InstructorId instructorId,
        UserId userId,
        decimal ratingValue,
        string? comment = null)
    {
        Result<Rating> ratingResult = Rating.Create(ratingValue, comment);
        if (ratingResult.IsFailure)
            return Result.Failure<InstructorRating>(ratingResult.Error);
        var instructorRating = new InstructorRating(InstructorRatingId.New(), instructorId, userId, ratingResult.Value);
        return Result.Success(instructorRating);
    }

    public Result Update(decimal newRatingValue, string? newComment = null)
    {
        Result<Rating> ratingResult = Rating.Create(newRatingValue, newComment);
        if (ratingResult.IsFailure)
            return ratingResult;

        Rating = ratingResult.Value;
        return Result.Success();
    }
}
