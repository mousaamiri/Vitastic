using Vitastic.Web.Models.DTOs.Course;

namespace Vitastic.Web.Models.ViewModels;


public class CourseCardViewModel(SimpleCourseDto course)
{
    public SimpleCourseDto Course { get; set; } = course;
    public string FormattedPrice { get; init; } = string.Empty;   // "۱۲۰,۰۰۰ تومان"
    public string LevelText { get; init; } = string.Empty;        // "مقدماتی"
    public string LevelCssClass { get; init; } = string.Empty;    // "badge-beginner"
    public string FormattedDuration { get; init; } = string.Empty; // "۱۲ ساعت"
    public bool IsFree => Course.Price == 0;
}

public class LevelOptionViewModel
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class SortOptionViewModel
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}
