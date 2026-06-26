using System.Linq.Expressions;
using System.Text.Json;
using FFMpegCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Vitastic.Web.Areas.AdminPanel.Controllers.Base;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Category;
using Vitastic.Web.Models.DTOs.Course;
using Vitastic.Web.Models.DTOs.Instructor;
using Vitastic.Web.Models.DTOs.Tag;
using Vitastic.Web.Models.ViewModels;

namespace Vitastic.Web.Areas.AdminPanel.Controllers;

public class CourseManagerController(ICourseService courseService, IInstructorService instructorService,
    ITagManagerService tagService, ICategoryManagerService categoryManager) : AdminController
{
    #region Index
    [HttpGet]
    public async Task<IActionResult> IndexAsync()
    {
        var response = await courseService.SearchCoursesAsync(new CourseFilterDto(1, 100));
        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] = response.Message;
            return View(new PaginatedData<SimpleCourseDto>());
        }
        return View(response.Data);
    }

    #endregion
    #region Create
    [HttpGet("Create")]
    public async Task<IActionResult> Create()
    {
        var createCoursePartialViewModel = new UpsertCoursePartialViewModel();
        IActionResult partialView = await InitUpsertView(createCoursePartialViewModel);
        return partialView;
    }
    [HttpPost("Upsert")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(UpsertCoursePartialViewModel model)
    {
        var createCourseRequest = new UpsertCreateCourseRequest(

            model.Title, model.Description, model.ShortDescription, model.Slug,
            model.Level, model.InstructorId, model.SelectTags, [model.CategoryId],
            false,
            model.CourseImageFile,
            model.CourseImageFile,
            model.CourseDemoVideoFile
            );
        ApiResponse response = await courseService.UpsertCourseByAdminAsync(createCourseRequest, model.CourseId);
        if (!response.IsSuccess)
            return BadRequest(new { success = false, message = response.Errors.Count > 0 ? response.Errors.First() : response.Message });
        return Ok(new { success = true, message = "دوره با موفقیت ایجاد شد." });
    }
    #endregion

    #region Update

    [HttpGet("{courseId:guid}/Update")]
    public async Task<IActionResult> Update(Guid courseId)
    {
        var viewModel = new UpsertCoursePartialViewModel();
        ApiResponse<CourseDto> course = await courseService.GetCourseByIdAsync(courseId, CancellationToken.None);
        if (!course.IsSuccess)
            return ReturnError(course, viewModel);
        viewModel.CourseId = course.Data!.Id;
        viewModel.CategoryId = course.Data!.CategoryIds.FirstOrDefault();
        viewModel.SelectTags = course.Data!.TagIds;
        viewModel.DemoVideoFilePath = course.Data!.DemoVideoName;
        viewModel.ImageFilePath = course.Data!.ImageName;
        viewModel.Title = course.Data!.Title;
        viewModel.Description = course.Data!.Description;
        viewModel.ShortDescription = course.Data!.ShortDescription;
        viewModel.Slug = course.Data!.Slug;
        viewModel.Level = course.Data!.Level;
        viewModel.InstructorId = course.Data!.InstructorId;
        IActionResult partialView = await InitUpsertView(viewModel);
        return partialView;
    }




    [HttpPost("{courseId:guid}/Update")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(Guid courseId, UpsertCoursePartialViewModel model)
    {
        var createCourseRequest = new UpsertCreateCourseRequest(
            model.Title, model.Description, model.ShortDescription, model.Slug,
            model.Level, model.InstructorId, model.SelectTags, [model.CategoryId],
            false,
            model.CourseImageFile,
            model.CourseImageFile,
            model.CourseDemoVideoFile
            );
        ApiResponse response = await courseService.UpsertCourseByAdminAsync(createCourseRequest);
        if (!response.IsSuccess)
            return BadRequest(new { success = false, message = response.Errors.Count > 0 ? response.Errors.First() : response.Message });
        return Ok(new { success = true, message = "دوره با موفقیت ایجاد شد." });
    }

    #endregion

    #region Usert Sections

    [HttpGet("{courseId:guid}/UpsertSections")]
    public async Task<IActionResult> UpsertSections(Guid courseId, CancellationToken token = default)
    {
        var viewModel = new UpsertSectionsViewModel();

        ApiResponse<CourseDto> courseResult = courseResult = await courseService.GetCourseByIdAsync(courseId, token);
        if (!courseResult.IsSuccess)
        {
            TempData["ErrorMessage"] =
                courseResult.Errors.Count > 0 ? courseResult.Errors.First() : courseResult.Message;
        }
        viewModel.CourseTitle = courseResult.Data!.Title;
        viewModel.Sections = courseResult.Data!.Sections;
        return View("_UpsertSections", viewModel);
    }
    [HttpPost("UpsertSections")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpsertSections(UpsertSectionsViewModel model, CancellationToken token = default)
    {
        foreach (SectionDto section in model.Sections)
        {
            TimeSpan sectionDuration = TimeSpan.Zero;
            foreach (EpisodeDto episode in section.Episodes)
            {
                if (episode.VideoFile is not null)
                {
                    episode.Duration = TimeSpan.FromSeconds((double)episode.DurationSeconds);
                }
                sectionDuration += episode.Duration;
                episode.IsFree = episode.Price > 0;
            }
            section.EpisodeCount = section.Episodes.Count;
            section.TotalDuration = sectionDuration;
        }

        ApiResponse response = await courseService.UpsertCourseSections(model.CourseId, model.Sections, token);
        if (!response.IsSuccess)
            return BadRequest(response.Errors.Count > 0 ? response.Errors.First() : response.Message);
        return Ok(model);
    }

    #endregion
    #region Hellpers
    private async Task<IActionResult>
        InitUpsertView(UpsertCoursePartialViewModel createCoursePartialViewModel)
    {
        PaginatedApiResponse<InstructorDto> instructors = await instructorService.GetTopInstructorsAsync(100, CancellationToken.None);
        if (!instructors.IsSuccess)
            return ReturnError(instructors, createCoursePartialViewModel);

        createCoursePartialViewModel.Instructors =
            instructors.Data!.Items.Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.FullName
            }).ToList();

        ApiResponse<PaginatedData<TagDto>> tags = await tagService.GetAllAsync(CancellationToken.None);
        if (!tags.IsSuccess)
            return ReturnError(tags, createCoursePartialViewModel);
        createCoursePartialViewModel.Tags =
            tags.Data!.Items.Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.Name
            }).ToList();

        ApiResponse<List<CategoryDetailDto>> categories = await categoryManager.GetAllAsync(CancellationToken.None);
        if (!categories.IsSuccess)
            return ReturnError(categories, createCoursePartialViewModel);

        createCoursePartialViewModel.Categories =
            categories.Data!.Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.Name
            }).ToList();
        return PartialView("_Upsert", createCoursePartialViewModel);
    }

    public IActionResult ReturnError(ApiResponse response,
        UpsertCoursePartialViewModel createCoursePartialViewModel)
    {
        TempData["ErrorMessage"] = response.Errors.Any() ? response.Errors.First() : response.Message;
        createCoursePartialViewModel.Instructors = [];
        return PartialView("_Upsert", createCoursePartialViewModel);
    }

    #endregion
}
