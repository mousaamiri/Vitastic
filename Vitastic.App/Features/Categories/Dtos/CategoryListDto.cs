namespace Vitastic.App.Features.Categories.Dtos
{
    public sealed record CategoryListDto
    {
        public Guid Id{get; init; }
        public string Name { get; init; } = null!;
        public string Slug { get; init; } = null!;
        public int DisplayOrder{get; init; }
        public bool IsActive{get; init; }
        public Guid? ParentId{get; init; }

        public CategoryListDto() { }
    }
}
