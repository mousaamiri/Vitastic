namespace Vitastic.Domain.Entities.Courses.Enums
{
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
}
