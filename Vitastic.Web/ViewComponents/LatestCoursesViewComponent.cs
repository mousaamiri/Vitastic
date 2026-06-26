using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models;
using Vitastic.Web.Models.ViewModels;
using SimpleCourseDto = Vitastic.Web.Models.DTOs.Course.SimpleCourseDto;

namespace Vitastic.Web.ViewComponents;

public class LatestCoursesViewComponent(ICourseService courseService):ViewComponent
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

// public class FooterViewComponent(ISettingService settingService) : ViewComponent
// {
//     public async Task<IViewComponentResult> InvokeAsync()
//     {
//         var settings = await settingService.GetFooterSettingsAsync();
//         return View(settings);
//     }
// }
// public class NotificationBellViewComponent(INotificationService notificationService) : ViewComponent
// {
//     public async Task<IViewComponentResult> InvokeAsync()
//     {
//         var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//         if (string.IsNullOrEmpty(userId)) return Content("");
//
//         var unreadCount = await notificationService.GetUnreadCountAsync(userId);
//         return View(unreadCount);
//     }
// }
// public class ShoppingCartWidgetViewComponent(ICartService cartService) : ViewComponent
// {
//     public async Task<IViewComponentResult> InvokeAsync()
//     {
//         var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//         if (string.IsNullOrEmpty(userId)) return View(0);
//
//         var itemCount = await cartService.GetCartItemCountAsync(userId);
//         return View(itemCount);
//     }
// }
