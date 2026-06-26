using Vitastic.App.Common.Abstractions.Files;

namespace Vitastic.App.Features.Courses.Dtos;

public sealed record SimpleCourseDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string ShortDescription { get; init; } = string.Empty;
    public string Slug { get; init; } = string.Empty;

    public string? ImageName { get; init; }
    public string? ThumbnailName { get; init; }

    public decimal Price { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Level { get; init; } = string.Empty;

    public bool HasCertificate { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }

    public TimeSpan TotalDuration { get; init; }
    public int FreeEpisodesCount { get; init; }
    public int TotalRatings { get; init; }
    public decimal AverageRating { get; set; }
    public bool IsPurchased { get; set; } = false;
    public bool IsInCart { get; set; }

    public Guid InstructorId { get; set; }
    public CourseInstructorDto Instructor { get; set; }
}

public sealed record CourseInstructorDto
{
    public string Name { get; set; } = string.Empty;
    public string Experties { get; set; } = string.Empty;
    public string Avatar { get; set; } = string.Empty;
    public int TotalRatings { get; set; }
    public decimal AverageRating { get; set; }
    public Guid Id { get; set; }
}

public sealed record CourseDto
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
    public CourseStatusDto Status { get; init; }
    public CourseLevelDto Level { get; init; }

    public bool HasCertificate { get; init; }
    public bool IsPublished { get; init; }

    public Guid InstructorId { get; init; }

    public DateTimeOffset? PublishedAt { get; init; }
    public DateTimeOffset? ArchivedAt { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset? UpdatedAt { get; init; }

    public int TotalSections { get; init; }
    public int TotalEpisodes { get; init; }
    public TimeSpan TotalDuration { get; init; }
    public int FreeEpisodesCount { get; init; }
    public bool HasFreeContent { get; init; }
    public bool IsPurchased { get; set; } = false;
    public bool IsInCart { get; set; }

    public List<Guid> TagIds { get; init; } = [];
    public List<Guid> CategoryIds { get; init; } = [];
    public List<SectionDto> Sections { get; init; } = [];
}

public sealed record SectionDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public int EpisodeCount { get; init; }
    public TimeSpan TotalDuration { get; init; }
    public List<EpisodeDto> Episodes { get; init; } = [];
}

public sealed record EpisodeDto
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public TimeSpan Duration { get; init; }
    public decimal Price { get; init; }
    public bool IsFree { get; init; }
    public IFile? VideoFile { get; set; }
    public string? VideoFileName { get; init; }
    public int DisplayOrder { get; init; }
}
