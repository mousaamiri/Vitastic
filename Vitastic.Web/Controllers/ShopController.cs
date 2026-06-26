using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Areas.Admin.Models.Course;
using Vitastic.Web.Models.Courses;
using Vitastic.Web.Models.Orders;
using Vitastic.Web.Services;

namespace Vitastic.Web.Controllers;

public class ShopController(ShopApiService shopService,OrderApiService orderApiService) : Controller
{
    // GET
    public async Task<IActionResult> Index(CoursesListFilterRequest model)
    {
        var shopViewModel = new ShopViewModel();
        shopViewModel.Courses =
            await shopService.GetShopCoursesListWithFiltersAsync(model);
        shopViewModel.Filters = (await shopService.GetShopFilters())!;
        return View(shopViewModel);
    }
    //Get Course Details by id
    [HttpGet("CourseDetails/{courseId:guid}")]
    public async Task<IActionResult> CourseDetails([FromRoute] Guid courseId)
    {
        CourseDetailUiModel? courseDetailModel =await shopService.GetShopCourseDetailsById(courseId);
        return View(courseDetailModel);
    }
    [HttpGet("Shop/Buy")]
    public async Task<IActionResult> Buy([FromQuery] Guid courseId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        await orderApiService.CreateOrderAsync(new CreateOrderRequest(userId, courseId));
        return RedirectToAction(nameof(Index), "Order","User");
    }
}
