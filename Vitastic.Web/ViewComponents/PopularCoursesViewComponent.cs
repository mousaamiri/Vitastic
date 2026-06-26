using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models;
using Vitastic.Web.Models.DTOs.Course;
using Vitastic.Web.Models.ViewModels;

namespace Vitastic.Web.ViewComponents;

public class PopularCoursesViewComponent(ICourseService courseService):ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(int count=6, CancellationToken ct = default)
    {

        PaginatedApiResponse<SimpleCourseDto> courses = await courseService.GetLastestCoursesAsync(count,ct);
        var courseViewModels = new PaginatedData<CourseCardViewModel>
        {
            Items = courses.Data!.Items.Select(i => i.ToCardViewModel()).ToList(),
            HasNextPage = courses.Data.HasNextPage,
            HasPreviousPage = courses.Data.HasPreviousPage,
            PageNumber = courses.Data.PageNumber,
            PageSize = courses.Data.PageSize,
            TotalCount = courses.Data.TotalCount,
            TotalPages = courses.Data.TotalPages,
        };
        return View(courseViewModels);
    }
}
