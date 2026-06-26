using Vitastic.Domain.Entities.Instructors.Enums;
using Vitastic.Domain.Entities.Instructors.Events;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Entities.Instructors;

public class Instructor : AggregateRoot<InstructorId>
{
    #region Fields

    private readonly List<InstructorRating> _ratings = [];

    #endregion

    #region Properties

    public UserId UserId { get; private set; }

    public FullName FullName { get; private set; }

    public UserAvatar Avatar { get; private set; }

    public InstructorBio Bio { get; private set; }

    public InstructorSkills Skills { get; private set; }

    public InstructorExpertise Expertise { get; private set; }

    public InstructorStatus Status { get; private set; }

    public IReadOnlyCollection<InstructorRating> Ratings => _ratings.AsReadOnly();

    public decimal AverageRating =>
        _ratings.Any() ? Math.Round(_ratings.Average(r => r.Rating.Value), 1) : 0;

    public int TotalRatings => _ratings.Count;

    #endregion

    #region EF Constructor

    protected Instructor() { }

    #endregion

    #region Private Constructor

    private Instructor(
        UserId userId,
        FullName fullName,
        UserAvatar avatar,
        InstructorBio bio,
        InstructorExpertise expertise)
        : base(InstructorId.New())
    {
        UserId = userId;
        FullName = fullName;
        Avatar = avatar;
        Bio = bio;
        Expertise = expertise;
        Skills = InstructorSkills.Create([]).Value;
        Status = InstructorStatus.Inactive;
    }

    #endregion

    #region Factory

    public static Result<Instructor> Create(
        UserId userId,
        FullName fullName,
        UserAvatar avatar,
        InstructorBio bio,
        InstructorExpertise expertise)
    {
        var instructor = new Instructor(
            userId,
            fullName,
            avatar,
            bio,
            expertise);

        instructor.RaiseDomainEvent(
             InstructorCreatedEvent.Create(instructor.Id, fullName));

        return Result.Success(instructor);
    }

    #endregion

    #region Business Behaviors

    public Result UpdateBio(InstructorBio newBio)
    {
        Bio = newBio;
        return Result.Success();
    }

    public Result UpdateExpertise(InstructorExpertise expertise)
    {
        Expertise = expertise;
        return Result.Success();
    }

    public Result AddSkill(InstructorSkill skill)
    {
        var newSkillsResult = Skills.Add(skill);

        if (newSkillsResult.IsFailure)
            return newSkillsResult.Error;

        Skills = newSkillsResult.Value;

        return Result.Success();
    }

    public Result RemoveSkill(InstructorSkill skill)
    {
        InstructorSkill? skillToRemove = Skills.Values
            .FirstOrDefault(s => s.Equals(skill));

        if (skillToRemove is null)
            return InstructorErrors.SkillNotFound;

        IEnumerable<InstructorSkill> updated = Skills.Values
            .Where(s => !s.Equals(skillToRemove));

        Result<InstructorSkills> result = InstructorSkills.Create(updated);

        if (result.IsFailure)
            return result.Error;

        Skills = result.Value;

        return Result.Success();
    }

    public Result AddRating(InstructorRating rating)
    {
        _ratings.Add(rating);
        return Result.Success();
    }

    public Result Activate()
    {
        if (Status == InstructorStatus.Active)
            return InstructorErrors.InstructorAlreadyActive;

        if (Status == InstructorStatus.Rejected)
            return InstructorErrors.CannotActivateRejectedInstructor;

        if (Status == InstructorStatus.Suspended)
            return InstructorErrors.CannotActivateSuspendedInstructor;

        Status = InstructorStatus.Active;

        RaiseDomainEvent( InstructorActivatedEvent.Create(Id));

        return Result.Success();
    }

    public Result Deactivate()
    {
        if (Status != InstructorStatus.Active)
            return InstructorErrors.InstructorIsNotActive;

        Status = InstructorStatus.Inactive;

        RaiseDomainEvent( InstructorDeactivatedEvent.Create(Id));

        return Result.Success();
    }

    public Result Suspend(string? reason = null)
    {
        if (Status == InstructorStatus.Suspended)
            return InstructorErrors.InstructorAlreadySuspended;

        if (Status == InstructorStatus.Rejected)
            return InstructorErrors.CannotSuspendRejectedInstructor;

        Status = InstructorStatus.Suspended;

        RaiseDomainEvent( InstructorSuspendedEvent.Create(Id, reason));

        return Result.Success();
    }

    public Result SubmitForApproval()
    {
        if (Status != InstructorStatus.Inactive &&
            Status != InstructorStatus.PendingApproval)
            return InstructorErrors.InvalidStateForApprovalSubmission;

        Status = InstructorStatus.PendingApproval;

        RaiseDomainEvent( InstructorSubmittedForApprovalEvent.Create(Id));

        return Result.Success();
    }

    public Result Reject(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            return InstructorErrors.RejectionReasonRequired;

        if (Status == InstructorStatus.Rejected)
            return InstructorErrors.InstructorAlreadyRejected;

        Status = InstructorStatus.Rejected;

        RaiseDomainEvent( InstructorRejectedEvent.Create(Id, reason));

        return Result.Success();
    }

    #endregion
}
