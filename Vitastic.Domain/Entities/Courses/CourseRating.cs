using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Domain.Entities.Courses;

public class CourseRating:BaseEntity<CourseRatingId>
{
    public CourseId CourseId { get; private set; }
    public UserId UserId { get; private set; }
    public Rating Rating { get; private set; }

    public Course Course { get; set; }
    public User User { get; set; }

    private CourseRating() { }

    private CourseRating(CourseRatingId id, CourseId courseId, UserId userId, Rating rating):base(id)
    {
        CourseId = courseId;
        UserId = userId;
        Rating =rating;
    }

    public static Result<CourseRating> Create(
        CourseId courseId,
        UserId userId,
        decimal ratingValue,
        string? comment = null)
    {
        Result<Rating> ratingResult = Rating.Create(ratingValue, comment);
        if (ratingResult.IsFailure)
            return Result.Failure<CourseRating>(ratingResult.Error);
        var courseRating = new CourseRating(CourseRatingId.New(), courseId, userId,ratingResult.Value);
        return Result.Success(courseRating);
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
