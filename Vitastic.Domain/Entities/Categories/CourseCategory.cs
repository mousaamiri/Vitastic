using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Models;

namespace Vitastic.Domain.Entities.Categories;

public class CourseCategory:BaseEntity<CourseCategoryId>
{
    public CourseId CourseId { get; private set; }
    public CategoryId CategoryId { get; private set; }
    public DateTimeOffset AssignedAt { get; private set; }=DateTimeOffset.UtcNow;

    public CourseCategory() { }//For ef
    private CourseCategory(CourseCategoryId id, CourseId courseId, CategoryId categoryId)
        : base(id)
    {
        CourseId = courseId;
        CategoryId = categoryId;
    }
    public static CourseCategory Create(CourseId courseId, CategoryId categoryId)
    {
        return new CourseCategory(CourseCategoryId.New(), courseId, categoryId);
    }
}

