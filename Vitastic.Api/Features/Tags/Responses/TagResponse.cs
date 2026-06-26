namespace Vitastic.Api.Features.Tags.Responses;

public sealed record TagResponse
{
    public Guid Id{get; init;}
    public string Name { get; init; } = null!;
    public int UsageCount{get; init;}
    public bool IsActive{get; init;}
    public TagResponse() { }
}
