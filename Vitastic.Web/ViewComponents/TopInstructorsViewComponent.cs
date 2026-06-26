using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Instructor;

namespace Vitastic.Web.ViewComponents
{
    public class TopInstructorsViewComponent(IInstructorService instructorService):ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int count=4, CancellationToken ct = default)
        {
            PaginatedApiResponse<InstructorDto> courses = await instructorService.GetTopInstructorsAsync(count,ct);
            return View(courses.Data);
        }
    }
}
