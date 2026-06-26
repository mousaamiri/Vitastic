namespace Vitastic.Web.Models.DTOs.Course;

public sealed record CourseFilterDto(
    int PageNumber = 1,
    int PageSize = 6,
    string? SearchTerm = null,
    Guid? InstructorId = null,
    string? CategoryName = null, // For display in active filters
    Guid? CategoryId = null,
    CourseLevelDto? Level = null,
    CourseStatusDto? Status = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    CourseSortBy? SortBy = null, // newest, popular, cheapest, expensive
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? HasCertificate = null,
    bool? IsFree = null
);
 #region Sort Enum
 public enum CourseSortBy
 {
     Newest = 0,                 // جدیدترین
     Oldest = 1,           // قدیمی‌ترین
     PriceAscending = 2,         // ارزان‌ترین
     PriceDescending = 3,        // گران‌ترین
     BestSelling = 4,            // پرفروش‌ترین
     HighestRated = 5,       // بیشترین امتیاز
     MostPopular = 6             // محبوب‌ترین (ترکیب فروش + امتیاز)
 }
 #endregion
