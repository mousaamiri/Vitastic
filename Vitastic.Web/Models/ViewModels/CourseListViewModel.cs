using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs;
using Vitastic.Web.Models.DTOs.Course;

namespace Vitastic.Web.Models.ViewModels;

public class CourseListViewModel
{
    // Filter parameters
    public CourseFilterDto Filter { get; set; } = new();

    // Results
    public PaginatedData<CourseCardViewModel> Courses { get; set; }

    // Filter options (for dropdowns)
    public List<CategoryViewModel> Categories { get; set; } = [];
    public List<LevelOptionViewModel> Levels { get; set; } = [];
    public List<SortOptionViewModel> SortOptions { get; set; } = [];
}
