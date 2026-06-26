namespace Vitastic.Web.Models.DTOs.Tag;

public sealed record TagDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = null!;
    public int UsageCount { get; init; }
    public bool IsActive { get; init; }
    public TagDto() { }
}
