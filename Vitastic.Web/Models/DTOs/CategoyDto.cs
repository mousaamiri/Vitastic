namespace Vitastic.Web.Models.DTOs
{
    public sealed record CategoryViewModel
    {
        public string Icon { get; set; } = "bi bi-code-slash";
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; }= null!;
        public string? Description { get; set; }
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
        public Guid? ParentCategoryId { get; set; }

        public string? ParentCategoryName { get; set; }
        public IReadOnlyList<CategoryViewModel> SubCategories { get; set; } = [];
    }
}
