namespace Vitastic.App.Features.Tags.Dtos;

public sealed record TagStatisticsDto(
    int TotalTags,
    int ActiveTags,
    int InactiveTags,
    int UsedTags,
    long TotalUsage,
    int MaxUsage,
    double AverageUsage);
