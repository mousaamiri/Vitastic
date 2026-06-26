using System.ComponentModel.DataAnnotations;

namespace Vitastic.Web.Areas.AdminPanel.Models.Course;

public sealed class CourseListItemModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public CourseUiLevel Level { get; set; }
    public string ThumbnailName { get; set; } = string.Empty;
    public CourseStatus Status { get; set; }
}

public enum CourseStatus
{
    [Display(Name = "پیش‌نویس")] Draft,

    [Display(Name = "منتشر شده")] Published,

    [Display(Name = "آرشیوشده")] Archived
}
