using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs;

namespace Vitastic.Web.ViewComponents;

public class CourseCategoryMenuViewComponent(ICategoryManagerService service):ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(int maxItems = 8)
    {
        PaginatedApiResponse<CategoryViewModel> categories = await service.GetTopCategoriesAsync(maxItems,CancellationToken.None);
        return View(categories.Data);
    }
}
