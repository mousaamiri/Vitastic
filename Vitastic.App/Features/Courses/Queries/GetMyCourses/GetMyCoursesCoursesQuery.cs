using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Entities.Courses.Enums;

namespace Vitastic.App.Features.Courses.Queries.GetMyCourses;

public sealed record GetMyCoursesCoursesQuery
    : IQuery<PaginatedResult<SimpleCourseDto>>
{
    public Guid UserId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; } = null;
    public Guid? InstructorId  { get; set; }= null;
    public Guid? CategoryId  { get; set; }= null;
    public CourseLevelDto? Level { get; set; } = null;
    public CourseStatusDto? Status { get; set; } = null;
    public DateTimeOffset? FromDate { get; set; }=null;
    public DateTimeOffset? ToDate { get; set; }=null;
    public CourseSortBy? SortBy  { get; set; }= null;
    public decimal? MinPrice { get; set; } = null;
    public decimal? MaxPrice { get; set; } = null;
    public bool? HasCertificate { get; set; } = null;
    public bool? IsFree { get; set; } = null;
}
