namespace Vitastic.Api.Features.Courses.Responses;
public enum CourseLevelResponse{
    Beginner = 1,
    Intermediate = 2,
    Advanced = 3,
    Expert = 4
}
public enum CourseStatusResponse
{
    Draft = 1,
    Published = 2,
    Archived = 3
}


public sealed record SimpleCourseResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ShortDescription { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;

    public string? ImageName { get; init; }
    public string? ThumbnailName { get; init; }

    public decimal Price { get; init; }
    public string Currency { get;init; } = "IRT";
    public string Level { get; init; }=string.Empty;

    public bool HasCertificate { get; init; }
    public DateTimeOffset?  UpdatedAt { get; init; }
    public DateTimeOffset  CreatedAt { get; init; }

    public TimeSpan TotalDuration { get; init; }
    public int FreeEpisodesCount { get; init; }
    public int TotalRatings { get; init; }
    public bool IsPurchased { get; set; } = false;
    public bool IsInCart { get; set; }

    public decimal AverageRating { get; init; }
    public Guid InstructorId { get; set; }
    public CourseInstructorResponse Instructor { get; set; }

}

public sealed record CourseInstructorResponse
{
    public string Name { get; set; }=string.Empty;
    public string Experties { get; set; }=string.Empty;
    public string Avatar { get; set; }=string.Empty;
    public int TotalRatings { get; set; }
    public decimal AverageRating { get;  set; }
    public Guid Id { get; set; }
}
public sealed record CourseResponse
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ShortDescription { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;

    public string? ImageName { get; init; }
    public string? ThumbnailName { get; init; }
    public string? DemoVideoName { get; init; }

    public decimal Price { get; init; }
    public CourseStatusResponse Status { get; init; }
    public CourseLevelResponse Level { get; init; }

    public bool HasCertificate { get; init; }
    public bool IsPublished { get; init; }

    public Guid InstructorId { get; init; }

    public DateTimeOffset?  PublishedAt { get; init; }
    public DateTimeOffset?  ArchivedAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset?  UpdatedAt { get; init; }

    public int TotalSections { get; init; }
    public int TotalEpisodes { get; init; }
    public TimeSpan TotalDuration { get; init; }
    public int FreeEpisodesCount { get; init; }
    public bool HasFreeContent { get; init; }
    public bool IsPurchased { get; set; } = false;
    public bool IsInCart { get; set; }

    public List<Guid> TagIds { get; init; } = [];
    public List<Guid> CategoryIds { get; init; } = [];
    public List<SectionResponse> Sections { get; init; } = [];
}

public sealed record SectionResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public int EpisodeCount { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public List<EpisodeResponse> Episodes { get; set; } = [];
}

public sealed record EpisodeResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public decimal Price { get; set; }
    public bool IsFree { get; set; }
    public IFormFile? VideoFile { get; set; }
    public string? VideoFileName { get; set; }
    public int DisplayOrder { get; set; }
}

