namespace Vitastic.App.Features.Tags.Dtos;

public record TagDto
{
    public Guid Id{get; init;}
    public string Name { get; init; } = null!;
    public int UsageCount{get; init;}
    public bool IsActive{get; init;}
    public TagDto() { }

    public TagDto(Guid id, string name, int usageCount, bool isActive)
    {
        Id = id;
        Name = name;
        UsageCount = usageCount;
        IsActive = isActive;
    }
}
