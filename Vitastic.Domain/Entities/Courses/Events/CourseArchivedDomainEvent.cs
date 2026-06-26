using System.Text.Json.Serialization;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Shared.Events;

namespace Vitastic.Domain.Entities.Courses.Events;

public sealed record CourseArchivedDomainEvent : DomainEvent
{
    public Guid CourseId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public CourseArchivedDomainEvent(Guid courseId)
    {
        CourseId = courseId;
    }

    public static CourseArchivedDomainEvent Create(CourseId courseId)
        => new(courseId.Value);
}

public sealed record CourseCreatedDomainEvent : DomainEvent
{
    public Guid CourseId { get; init; }
    public string CourseTitle { get; init; }
    public Guid InstructorId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public CourseCreatedDomainEvent(Guid courseId, string courseTitle, Guid instructorId)
    {
        CourseId = courseId;
        CourseTitle = courseTitle;
        InstructorId = instructorId;
    }

    public static CourseCreatedDomainEvent Create(CourseId courseId, CourseTitle courseTitle, InstructorId instructorId)
        => new(courseId.Value, courseTitle.Value, instructorId.Value);
}

public sealed record CoursePublishedDomainEvent : DomainEvent
{
    public Guid CourseId { get; init; }
    public string CourseTitle { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public CoursePublishedDomainEvent(Guid courseId, string courseTitle)
    {
        CourseId = courseId;
        CourseTitle = courseTitle;
    }

    public static CoursePublishedDomainEvent Create(CourseId courseId, CourseTitle courseTitle)
        => new(courseId.Value, courseTitle.Value);
}

public sealed record CourseUnpublishedDomainEvent : DomainEvent
{
    public Guid CourseId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public CourseUnpublishedDomainEvent(Guid courseId)
    {
        CourseId = courseId;
    }

    public static CourseUnpublishedDomainEvent Create(CourseId courseId)
        => new(courseId.Value);
}

public sealed record EpisodeAddedDomainEvent : DomainEvent
{
    public Guid CourseId { get; init; }
    public Guid SectionId { get; init; }
    public Guid EpisodeId { get; init; }
    public string EpisodeTitle { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public EpisodeAddedDomainEvent(Guid courseId, Guid sectionId, Guid episodeId, string episodeTitle)
    {
        CourseId = courseId;
        SectionId = sectionId;
        EpisodeId = episodeId;
        EpisodeTitle = episodeTitle;
    }

    public static EpisodeAddedDomainEvent Create(CourseId courseId, SectionId sectionId, EpisodeId episodeId, EpisodeTitle episodeTitle)
        => new(courseId.Value, sectionId.Value, episodeId.Value, episodeTitle.Value);
}

public sealed record SectionAddedDomainEvent : DomainEvent
{
    public Guid CourseId { get; init; }
    public Guid SectionId { get; init; }
    public string SectionTitle { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public SectionAddedDomainEvent(Guid courseId, Guid sectionId, string sectionTitle)
    {
        CourseId = courseId;
        SectionId = sectionId;
        SectionTitle = sectionTitle;
    }

    public static SectionAddedDomainEvent Create(CourseId courseId, SectionId sectionId, SectionTitle sectionTitle)
        => new(courseId.Value, sectionId.Value, sectionTitle.Value);
}

public sealed record SectionRemovedDomainEvent : DomainEvent
{
    public Guid CourseId { get; init; }
    public Guid SectionId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public SectionRemovedDomainEvent(Guid courseId, Guid sectionId)
    {
        CourseId = courseId;
        SectionId = sectionId;
    }

    public static SectionRemovedDomainEvent Create(CourseId courseId, SectionId sectionId)
        => new(courseId.Value, sectionId.Value);
}
