using Vitastic.Web.Models.DTOs.Course;

namespace Vitastic.Web.Models.ViewModels;

public sealed class UpsertSectionsViewModel
{
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public List<SectionDto> Sections { get; set; } = [];
}
