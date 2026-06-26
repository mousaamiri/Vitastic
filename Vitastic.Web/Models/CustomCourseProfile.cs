using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Models.DTOs.Course;
using Vitastic.Web.Models.ViewModels;

namespace Vitastic.Web.Models;

#region Course Mapper - DTO to ViewModel

public static class CustomCourseMapper
{
    public static CourseCardViewModel ToCardViewModel(this SimpleCourseDto dto)
    {
        return new CourseCardViewModel(dto)
        {
            FormattedPrice = FormatPrice(dto.Price),
            LevelText = GetLevelText(dto.Level),
            LevelCssClass = GetLevelCssClass(dto.Level),
            FormattedDuration = FormatDuration(dto.TotalDuration),
        };
    }

    public static CourseListViewModel ToListViewModel(
        this PaginatedApiResponse<SimpleCourseDto> result,
        CourseFilterDto filter)
    {
        return new CourseListViewModel
        {
            Filter = filter,
            Courses = new PaginatedData<CourseCardViewModel>
            {
                Items = result.Data!.Items.Select(i => i.ToCardViewModel()).ToList(),
                HasNextPage = result.Data.HasNextPage,
                HasPreviousPage = result.Data.HasPreviousPage,
                PageNumber = result.Data.PageNumber,
                PageSize = result.Data.PageSize,
                TotalCount = result.Data.TotalCount,
                TotalPages = result.Data.TotalPages,
            },
            Levels = GetDefaultLevels(),
            SortOptions = GetDefaultSortOptions()
        };
    }

    #region Private Helpers

    private static string FormatPrice(decimal price)
    {
        if (price == 0) return "رایگان";
        return $"{price:N0} تومان";
    }

    private static string GetLevelText(string level)
    {
        return level.ToLower() switch
        {
            "beginner" => "مقدماتی",
            "intermediate" => "متوسط",
            "advanced" => "پیشرفته",
            "expert" => "تخصصی",
            _ => "نامشخص"
        };
    }

    private static string GetLevelCssClass(string level)
    {
        return level.ToLower() switch
        {
            "beginner" => "badge-beginner",
            "intermediate" => "badge-intermediate",
            "advanced" => "badge-advanced",
            "expert" => "badge-expert",
            _ => "badge-default"
        };
    }

    private static string FormatDuration(TimeSpan duration)
    {
        if (duration.TotalHours >= 1)
            return $"{(int)duration.TotalHours} ساعت و {duration.Minutes} دقیقه";
        return $"{duration.Minutes} دقیقه";
    }

    private static List<LevelOptionViewModel> GetDefaultLevels()
    {
        return
        [
            new LevelOptionViewModel { Value = "beginner", Text = "مقدماتی" },
            new LevelOptionViewModel { Value = "intermediate", Text = "متوسط" },
            new LevelOptionViewModel { Value = "advanced", Text = "پیشرفته" },
            new LevelOptionViewModel { Value = "expert", Text = "تخصصی" }
        ];
    }

    private static List<SortOptionViewModel> GetDefaultSortOptions()
    {
        return
        [
            new SortOptionViewModel { Value = "Newest", Text = "جدیدیترین" },
            new SortOptionViewModel { Value = "Oldest", Text = "قدیمی‌ترین" },
            new SortOptionViewModel { Value = "PriceAsc", Text = "ارزان‌ترین" },
            new SortOptionViewModel { Value = "PriceDesc", Text = "گران‌ترین" },
            new SortOptionViewModel { Value = "MostSold", Text = "پرفروش‌ترین" },
            new SortOptionViewModel { Value = "MostRated", Text = "بیشترین امتیاز" },
            new SortOptionViewModel { Value = "MostPopular", Text = "محبوب‌ترین (ترکیب فروش + امتیاز)" }
        ];
    }

    #endregion
}
#endregion
