using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vitastic.Web.Models.DTOs.Course;
using Vitastic.Web.Models.DTOs.Instructor;
using Vitastic.Web.Models.DTOs.Order;

namespace Vitastic.Web.Models.ViewModels;

public class UpsertCoursePartialViewModel
{
    public Guid? CourseId { get; set; }
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string Description { get; set; } = string.Empty;
    [Required] public string ShortDescription { get; set; } = string.Empty;
    [Required] public string Slug { get; set; } = string.Empty;
    [Required] public CourseLevelDto Level { get; set; }
    [Required] public Guid InstructorId { get; set; }
    [Required] public Guid CategoryId { get; set; }
    [Required] public IFormFile CourseImageFile { get; set; }
    public string? ImageFilePath { get; set; }
    public string?  DemoVideoFilePath { get; set; }
    [Required]
    [DataType(DataType.Upload)]
    //[RequestSizeLimit(50 * 1024 * 1024)]
    public IFormFile CourseDemoVideoFile { get; set; }
    [Required]public List<Guid> SelectTags { get; set; }



    public List<SelectListItem> Instructors { get; set; } = [];
    public List<SelectListItem> Categories { get; set; } = [];
    public List<SelectListItem> Tags { get; set; } = [];
    public List<SelectListItem> Levels { get; set; } = GetAvailablePaymentMethods();


    private static List<SelectListItem> GetAvailablePaymentMethods() =>
    [
        new()
    {
        Value = nameof(CourseLevelDto.Beginner),
        Text = "مقدماتی"
    },
    new()
    {
        Value = nameof(CourseLevelDto.Intermediate),
        Text = "متوسط"
    },
        new()
        {
            Value = nameof(CourseLevelDto.Advanced),
            Text = "پیشرفته"
        }, new()
        {
            Value =nameof(CourseLevelDto.Expert),
            Text = "حرفه ای"
        }
    ];
}

