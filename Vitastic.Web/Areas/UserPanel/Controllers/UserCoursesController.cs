using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Areas.UserPanel.Controllers.Base;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models;
using Vitastic.Web.Models.DTOs.Course;
using Vitastic.Web.Models.ViewModels;

namespace Vitastic.Web.Areas.UserPanel.Controllers;

public class UserCoursesController(ICourseService courseService, ILogger<UserCoursesController> logger)
    : UserController
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] CourseFilterDto filter)
    {
        PaginatedApiResponse<SimpleCourseDto> result =
            await courseService.GetMyCoursesAsync(filter, CancellationToken.None);

        var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

        // Handle AJAX error
        if (isAjax)
        {
            if (!result.IsSuccess)
            {
                return Json(new
                {
                    success = false,
                    message = result.Errors?.FirstOrDefault()
                              ?? result.Message
                              ?? "خطا در دریافت لیست دوره‌ها"
                });
            }
        }

        // Normal (non-AJAX)
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Errors?.FirstOrDefault()
                                ?? result.Message
                                ?? "خطا در دریافت لیست دوره‌ها";
            return View(new CourseListViewModel { Filter = filter });
        }

        CourseListViewModel viewModel = result.ToListViewModel(filter);

        // Load categories into sidebar
        var categories = await courseService.GetCategoriesForFilterAsync(CancellationToken.None);
        if (categories.IsSuccess)
            viewModel.Categories = categories.Data!.Items;

        // Return partial for AJAX
        if (isAjax)
            return PartialView("Common/_CourseGrid", viewModel);

        return View(viewModel);
    }

}
