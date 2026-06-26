using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vitastic.Web.Areas.AdminPanel.Models.Course.Category;
using Vitastic.Web.Areas.AdminPanel.Models.Course.Tag;
using Vitastic.Web.Areas.AdminPanel.Models.Users;

namespace Vitastic.Web.Areas.AdminPanel.Models.Course;

public class CourseDetailUiModel
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string ShortDescription { get; set; } = "";
    public string? ImageName { get; set; } = "";
    public string? ThumbnailName { get; set; } = "";
    public string DemoVideoName { get; set; } = "";
    public string Slug { get; set; } = "";
    public bool HasCertificate { get; set; }
    public decimal Price { get; set; }
    public int Status { get; set; }
    public int Level { get; set; }
    public string InstructorFullName { get; set; } = "";
    public string InstructorAvatar { get; set; } = "";

    public string InstructorUserName { get; set; } = "";

    // send image 
    public IFormFile? ImageFile { get; set; }


    //User chooses
    public Guid InstructorId { get; set; }
    public string SelectedTags { get; set; } = "";
    public List<Guid> SelectedCategoryIds { get; set; } = [];

    public IEnumerable<TagListItemModel> Tags { get; set; } = [];
    public IEnumerable<CategoryListItemModel> Categories { get; set; } = [];


    //For View
    public IEnumerable<SelectListItem> SelectCategories { get; set; } = [];
    public IEnumerable<TagDetilUiModel> SelectTags { get; set; } = [];
    public List<SectionUiModel> Sections { get; set; } = [];

    public IEnumerable<SelectListItem>? LevelsList { get; set; } = [];
    public IEnumerable<SelectListItem>? StatusList { get; set; } = [];

    //for bind select
    public IEnumerable<SelectListItem>? InstructorSelectList { get; set; } = [];
}

public class InstructorUiModel
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = "";
}

public class SectionUiModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public List<EpisodeUiModel> Episodes { get; set; } = new();
}

public class EpisodeUiModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public TimeSpan Duration { get; set; }
    public string VideoUrl { get; set; } = "";
}

public enum CourseUiLevel
{
    [Display(Name = "مبتدی")] Beginner,

    [Display(Name = "متوسط")] Intermediate,

    [Display(Name = "پیشرفته")] Advanced
}
