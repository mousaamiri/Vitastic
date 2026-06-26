using System.Collections.ObjectModel;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Api.Extensions;
using Vitastic.Api.Features.Base;
using Vitastic.Api.Features.Courses.Requests;
using Vitastic.Api.Features.Courses.Responses;
using Vitastic.Api.Wrapper;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Courses.Commands.AddCategory;
using Vitastic.App.Features.Courses.Commands.AddSection;
using Vitastic.App.Features.Courses.Commands.AddSectionEpisode;
using Vitastic.App.Features.Courses.Commands.AddTag;
using Vitastic.App.Features.Courses.Commands.Archive;
using Vitastic.App.Features.Courses.Commands.ChangeInstructor;
using Vitastic.App.Features.Courses.Commands.Create;
using Vitastic.App.Features.Courses.Commands.CreateByAdmin;
using Vitastic.App.Features.Courses.Commands.DisableCertificate;
using Vitastic.App.Features.Courses.Commands.EnableCertificate;
using Vitastic.App.Features.Courses.Commands.Publish;
using Vitastic.App.Features.Courses.Commands.RemoveCategory;
using Vitastic.App.Features.Courses.Commands.RemoveDemo;
using Vitastic.App.Features.Courses.Commands.RemoveSection;
using Vitastic.App.Features.Courses.Commands.RemoveSectionEpisode;
using Vitastic.App.Features.Courses.Commands.RemoveTag;
using Vitastic.App.Features.Courses.Commands.ReorderSections;
using Vitastic.App.Features.Courses.Commands.SetCategoryList;
using Vitastic.App.Features.Courses.Commands.SetDemo;
using Vitastic.App.Features.Courses.Commands.SetImage;
using Vitastic.App.Features.Courses.Commands.SetTagList;
using Vitastic.App.Features.Courses.Commands.Unpublish;
using Vitastic.App.Features.Courses.Commands.UpdateByAdmin;
using Vitastic.App.Features.Courses.Commands.UpdateDescription;
using Vitastic.App.Features.Courses.Commands.UpdateDetails;
using Vitastic.App.Features.Courses.Commands.UpdateLevel;
using Vitastic.App.Features.Courses.Commands.UpdateSectionEpisode;
using Vitastic.App.Features.Courses.Commands.UpdateSectionTitle;
using Vitastic.App.Features.Courses.Commands.UpdateSlug;
using Vitastic.App.Features.Courses.Commands.UpdateTitle;
using Vitastic.App.Features.Courses.Commands.UpsertCourseSections;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.App.Features.Courses.Queries.GetById;
using Vitastic.App.Features.Courses.Queries.GetBySlug;
using Vitastic.App.Features.Courses.Queries.GetCourseTitle;
using Vitastic.App.Features.Courses.Queries.GetCourseWithSections;
using Vitastic.App.Features.Courses.Queries.GetInstructorCourses;
using Vitastic.App.Features.Courses.Queries.GetMyCourses;
using Vitastic.App.Features.Courses.Queries.List;
using Vitastic.App.Features.Courses.Queries.Search;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Api.Features.Courses;

[Authorize]
[ApiController]
[Route("api/v1/courses")]
[Produces("application/json")]
public class CoursesController(
    IMediator mediator,
    IMapper mapper,
    ILogger<CoursesController> logger) : ControllerBase
{
    #region ==================== CREATE COURSE BY ADMIN ====================

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("by-admin")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<Guid>>> CreateCourseByAdmin(
        [FromForm] CreateCourseByAdminRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Creating course by admin — Title: {Title}, InstructorId: {InstructorId}, CategoryCount: {CategoryCount}",
            request.Title, request.InstructorId, request.CategoryIds?.Count ?? 0);

        var command = new CreateCourseByAdminCommand(
            request.Title,
            request.Description,
            request.ShortDescription,
            request.Slug,
            (CourseLevelDto)request.Level,
            request.InstructorId,
            request.TagIds,
            request.CategoryIds, request.HasCertificate,
            request.ImageFile is null? null:new FormFileAdapter(request.ImageFile),
            request.ThumbnailFile==null?null:new FormFileAdapter(request.ThumbnailFile),
            request.DemoVideoFile is null? null: new FormFileAdapter(request.DemoVideoFile));

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Create course by admin failed — {ErrorCode}: {ErrorMessage}",
                result.Error.Code, result.Error.Message);

            return result.ToApiResponse<Guid>("ایجاد دوره توسط ادمین انجام نشد.");
        }

        logger.LogInformation(
            "Course created by admin successfully — CourseId: {CourseId}",
            result.Value);

        return result.ToApiResponse(t => t, "دوره توسط ادمین با موفقیت ایجاد شد.");
    }

    #endregion

    #region ==================== UPDATE COURSE BY ADMIN ====================
    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{courseId:guid}/by-admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateCourseByAdmin(
        [FromRoute] Guid courseId,
        [FromForm] UpdateCourseByAdminRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Updating course by admin — CourseId: {CourseId}, Title: {Title}, InstructorId: {InstructorId}",
            courseId, request.Title, request.InstructorId);

        var command = new UpdateCourseByAdminCommand(
            courseId,
            request.Title,
            request.Description,
            request.ShortDescription,
            request.Slug,
            (CourseLevelDto)request.Level,
            request.InstructorId,
            request.TagIds,
            request.CategoryIds,
            request.HasCertificate,
            request.ImageFile is null? null:new FormFileAdapter(request.ImageFile),
            request.ThumbnailFile==null?null:new FormFileAdapter(request.ThumbnailFile),
            request.DemoVideoFile is null? null: new FormFileAdapter(request.DemoVideoFile));

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Update course by admin failed — CourseId: {CourseId}, {ErrorCode}: {ErrorMessage}",
                courseId, result.Error.Code, result.Error.Message);

            return BadRequest(result.ToApiResponse("به‌روزرسانی دوره توسط ادمین انجام نشد."));
        }

        logger.LogInformation("Course updated by admin successfully — CourseId: {CourseId}", courseId);
        return Ok(result.ToApiResponse("دوره توسط ادمین با موفقیت به‌روزرسانی شد."));
    }

    #endregion

    #region ================== UPDATE COURSE SETIONS ====================

    [HttpPost("{courseId:guid}/sections/upsert")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> UpdateCourseSections(
        [FromRoute] Guid courseId,
        [FromForm] UpsertCourseSections request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Updating course sections — CourseId: {CourseId}, SectionsCount: {SectionsCount}",
            courseId, request.Sections?.Count ?? 0);

        List<SectionDto> sections = mapper.Map<List<SectionDto>>(request.Sections);
        var command = new UpsertCourseSectionsCommand(courseId, sections);

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Update course sections failed — CourseId: {CourseId}, {ErrorCode}: {ErrorMessage}",
                courseId, result.Error.Code, result.Error.Message);

            return BadRequest(result.ToApiResponse("به‌روزرسانی بخش‌های دوره انجام نشد."));
        }

        logger.LogInformation("Course sections updated successfully — CourseId: {CourseId}", courseId);
        return Ok(result.ToApiResponse("بخش‌های دوره با موفقیت به‌روزرسانی شد."));
    }


    #endregion
    #region Create Course

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse<Guid>> Create(
        [FromBody] CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating course: {Title}", request.Title);

        CreateCourseCommand command = mapper.Map<CreateCourseCommand>(request);
        Result<Guid> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to create course: {Error}", result.Error);
            return result.ToApiResponse<Guid>("خطا در ایجاد دوره");
        }

        logger.LogInformation("Course created: {CourseId}", result.Value);

        return result.ToApiResponse(
            id => id,
            "ایجاد دوره با موفقیت انجام شد");
    }

    #endregion

    #region Add Course Category

    [HttpPost("{courseId:guid}/categories")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> AddCategory(
        [FromRoute] Guid courseId,
        [FromBody] AddCourseCategoryRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding category {CategoryId} to course {CourseId}", request.CategoryId, courseId);

        AddCourseCategoryCommand command =
            mapper.Map<AddCourseCategoryCommand>(request) with { CourseId = courseId };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to add category: {Error}", result.Error);
            return result.ToApiResponse("خطا در افزودن دسته‌بندی به دوره");
        }

        logger.LogInformation("Category {CategoryId} added to course {CourseId}", request.CategoryId, courseId);

        return result.ToApiResponse("دسته‌بندی با موفقیت به دوره اضافه شد");
    }

    #endregion

    #region Add Section

    [HttpPost("{courseId:guid}/sections/add")]
    [ProducesResponseType(typeof(ApiResponse<SectionResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<SectionResponse>> AddSection(
        [FromRoute] Guid courseId,
        [FromBody] AddCourseSectionRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding section to course {CourseId}", courseId);

        AddCourseSectionCommand command =
            mapper.Map<AddCourseSectionCommand>(request) with { CourseId = courseId };

        Result<SectionDto> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to add section: {Error}", result.Error);
            return result.ToApiResponse<SectionResponse>("خطا در افزودن بخش به دوره");
        }

        logger.LogInformation("Section added to course {CourseId}", courseId);

        return result.ToApiResponse(
            mapper.Map<SectionResponse>,
            "بخش با موفقیت به دوره اضافه شد");
    }

    #endregion

    #region Add Episode

    [HttpPost("{courseId:guid}/sections/{sectionId:guid}/episodes")]
    [ProducesResponseType(typeof(ApiResponse<EpisodeResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<EpisodeResponse>> AddEpisode(
        [FromRoute] Guid courseId,
        [FromRoute] Guid sectionId,
        [FromForm] AddCourseEpisodeRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding episode to section {SectionId} of course {CourseId}", sectionId, courseId);

        AddCourseEpisodeCommand command = mapper.Map<AddCourseEpisodeCommand>(request) with
        {
            CourseId = courseId,
            SectionId = sectionId,
            VideoFile = new FormFileAdapter(request.VideoFile)
        };

        Result<EpisodeDto> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to add episode: {Error}", result.Error);
            return result.ToApiResponse<EpisodeResponse>("خطا در افزودن اپیزود به بخش");
        }

        logger.LogInformation("Episode added to section {SectionId}", sectionId);

        return result.ToApiResponse(
            mapper.Map<EpisodeResponse>,
            "اپیزود با موفقیت اضافه شد");
    }

    #endregion

    #region Add Tag

    [HttpPost("{courseId:guid}/tags")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> AddTag(
        [FromRoute] Guid courseId,
        [FromBody] AddCourseTagRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding tag {TagId} to course {CourseId}", request.TagId, courseId);

        AddCourseTagCommand command =
            mapper.Map<AddCourseTagCommand>(request) with { CourseId = courseId };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to add tag: {Error}", result.Error);
            return result.ToApiResponse("خطا در افزودن تگ به دوره");
        }

        logger.LogInformation("Tag {TagId} added to course {CourseId}", request.TagId, courseId);

        return result.ToApiResponse("تگ با موفقیت به دوره اضافه شد");
    }

    #endregion

    #region Archive Course

    [HttpPatch("{courseId:guid}/archive")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Archive(
        [FromRoute] Guid courseId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Archiving course {CourseId}", courseId);

        Result result = await mediator.Send(new ArchiveCourseCommand(courseId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to archive course: {Error}", result.Error);
            return result.ToApiResponse("خطا در آرشیو کردن دوره");
        }

        logger.LogInformation("Course {CourseId} archived", courseId);

        return result.ToApiResponse("دوره با موفقیت آرشیو شد");
    }

    #endregion

    #region Change Course Instructor

    [HttpPatch("{courseId:guid}/instructor")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> ChangeInstructor(
        [FromRoute] Guid courseId,
        [FromBody] ChangeCourseInstructorRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Changing instructor for course {CourseId} to {InstructorId}",
            courseId,
            request.InstructorId);

        ChangeCourseInstructorCommand command =
            mapper.Map<ChangeCourseInstructorCommand>(request) with { CourseId = courseId };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to change instructor: {Error}", result.Error);
            return result.ToApiResponse("خطا در تغییر مدرس دوره");
        }

        logger.LogInformation("Instructor changed for course {CourseId}", courseId);

        return result.ToApiResponse("مدرس دوره با موفقیت تغییر کرد");
    }

    #endregion

    #region Disable Course Certificate

    [HttpPatch("{courseId:guid}/certificate/disable")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> DisableCertificate(
        [FromRoute] Guid courseId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Disabling certificate for course {CourseId}", courseId);

        Result result = await mediator.Send(new DisableCourseCertificateCommand(courseId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to disable certificate: {Error}", result.Error);
            return result.ToApiResponse("خطا در غیرفعال‌سازی گواهی دوره");
        }

        logger.LogInformation("Certificate disabled for course {CourseId}", courseId);

        return result.ToApiResponse("گواهی دوره با موفقیت غیرفعال شد");
    }

    #endregion

    #region Enable Course Certificate

    [HttpPatch("{courseId:guid}/certificate/enable")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> EnableCertificate(
        [FromRoute] Guid courseId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Enabling certificate for course {CourseId}", courseId);

        Result result = await mediator.Send(new EnableCourseCertificateCommand(courseId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to enable certificate: {Error}", result.Error);
            return result.ToApiResponse("خطا در فعال‌سازی گواهی دوره");
        }

        logger.LogInformation("Certificate enabled for course {CourseId}", courseId);

        return result.ToApiResponse("گواهی دوره با موفقیت فعال شد");
    }

    #endregion

    #region Publish Course

    [HttpPatch("{courseId:guid}/publish")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Publish(
        [FromRoute] Guid courseId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing course {CourseId}", courseId);

        Result result = await mediator.Send(new PublishCourseCommand(courseId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to publish course: {Error}", result.Error);
            return result.ToApiResponse("خطا در انتشار دوره");
        }

        logger.LogInformation("Course {CourseId} published", courseId);

        return result.ToApiResponse("دوره با موفقیت منتشر شد");
    }

    #endregion

    #region Remove Course Category

    [HttpDelete("{courseId:guid}/categories/{categoryId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> RemoveCategory(
        [FromRoute] Guid courseId,
        [FromRoute] Guid categoryId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing category {CategoryId} from course {CourseId}", categoryId, courseId);

        Result result = await mediator.Send(
            new RemoveCourseCategoryCommand(courseId, categoryId),
            cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to remove category: {Error}", result.Error);
            return result.ToApiResponse("خطا در حذف دسته‌بندی از دوره");
        }

        logger.LogInformation("Category removed from course {CourseId}", courseId);

        return result.ToApiResponse("دسته‌بندی با موفقیت از دوره حذف شد");
    }

    #endregion

    #region Remove Course Demo

    [HttpDelete("{courseId:guid}/demo")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> RemoveDemo(
        [FromRoute] Guid courseId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing demo video from course {CourseId}", courseId);

        Result result = await mediator.Send(new RemoveCourseDemoVideoCommand(courseId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to remove demo: {Error}", result.Error);
            return result.ToApiResponse("خطا در حذف ویدیو دمو دوره");
        }

        logger.LogInformation("Demo removed from course {CourseId}", courseId);

        return result.ToApiResponse("ویدیو دمو با موفقیت حذف شد");
    }

    #endregion

    #region Remove Course Section

    [HttpDelete("{courseId:guid}/sections/{sectionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> RemoveSection(
        [FromRoute] Guid courseId,
        [FromRoute] Guid sectionId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing section {SectionId} from course {CourseId}", sectionId, courseId);

        Result result = await mediator.Send(
            new RemoveCourseSectionCommand(courseId, sectionId),
            cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to remove section: {Error}", result.Error);
            return result.ToApiResponse("خطا در حذف بخش دوره");
        }

        logger.LogInformation("Section removed from course {CourseId}", courseId);

        return result.ToApiResponse("بخش دوره با موفقیت حذف شد");
    }

    #endregion

    #region Remove Course Episode

    [HttpDelete("{courseId:guid}/sections/{sectionId:guid}/episodes/{episodeId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> RemoveEpisode(
        [FromRoute] Guid courseId,
        [FromRoute] Guid sectionId,
        [FromRoute] Guid episodeId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing episode {EpisodeId} from section {SectionId}", episodeId, sectionId);

        RemoveCourseEpisodeCommand command = new(courseId, sectionId, episodeId);

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to remove episode: {Error}", result.Error);
            return result.ToApiResponse("خطا در حذف اپیزود");
        }

        logger.LogInformation("Episode removed");

        return result.ToApiResponse("اپیزود با موفقیت حذف شد");
    }

    #endregion

    #region Remove Tag

    [HttpDelete("{courseId:guid}/tags/{tagId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> RemoveTag(
        [FromRoute] Guid courseId,
        [FromRoute] Guid tagId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing tag {TagId} from course {CourseId}", tagId, courseId);

        RemoveCourseTagCommand command = new(courseId, tagId);
        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to remove tag: {Error}", result.Error);
            return result.ToApiResponse("خطا در حذف تگ از دوره");
        }

        logger.LogInformation("Tag removed from course {CourseId}", courseId);
        return result.ToApiResponse("تگ با موفقیت از دوره حذف شد");
    }

    #endregion

    #region Reorder Sections

    [HttpPatch("{courseId:guid}/sections/reorder")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> ReorderSections(
        [FromRoute] Guid courseId,
        [FromBody] ReorderCourseSectionsRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Reordering sections for course {CourseId}", courseId);

        ReorderCourseSectionsCommand command = mapper.Map<ReorderCourseSectionsCommand>(request) with
        {
            CourseId = courseId
        };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to reorder sections: {Error}", result.Error);
            return result.ToApiResponse("خطا در مرتب‌سازی بخش‌های دوره");
        }

        logger.LogInformation("Sections reordered for course {CourseId}", courseId);
        return result.ToApiResponse("بخش‌ها با موفقیت مرتب شدند");
    }

    #endregion

    #region Set Categories

    [HttpPut("{courseId:guid}/categories")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> SetCategories(
        [FromRoute] Guid courseId,
        [FromBody] SetCourseCategoriesRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting categories for course {CourseId}", courseId);

        SetCourseCategoriesCommand command = mapper.Map<SetCourseCategoriesCommand>(request) with
        {
            CourseId = courseId
        };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to set categories: {Error}", result.Error);
            return result.ToApiResponse("خطا در تنظیم دسته‌بندی‌های دوره");
        }

        logger.LogInformation("Categories set for course {CourseId}", courseId);
        return result.ToApiResponse("دسته‌بندی‌ها با موفقیت ثبت شدند");
    }

    #endregion

    #region Set Demo

    [HttpPut("{courseId:guid}/demo")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> SetDemo(
        [FromRoute] Guid courseId,
        [FromForm] SetCourseDemoVideoRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting demo video for course {CourseId}", courseId);

        var command = new SetCourseDemoVideoCommand(courseId, new FormFileAdapter(request.VideoFile));
        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to set demo: {Error}", result.Error);
            return result.ToApiResponse("خطا در تنظیم ویدیو دمو");
        }

        logger.LogInformation("Demo set for course {CourseId}", courseId);
        return result.ToApiResponse("ویدیو دمو با موفقیت ثبت شد");
    }

    #endregion

    #region Set Image

    [HttpPut("{courseId:guid}/image")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> SetImage(
        [FromRoute] Guid courseId,
        [FromForm] SetCourseImageRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting image for course {CourseId}", courseId);

        SetCourseImageCommand command = new()
        {
            CourseId = courseId,
            ImageFile = new FormFileAdapter(request.ImageFile)
        };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to set image: {Error}", result.Error);
            return result.ToApiResponse("خطا در تنظیم تصویر دوره");
        }

        logger.LogInformation("Image set for course {CourseId}", courseId);
        return result.ToApiResponse("تصویر دوره با موفقیت ثبت شد");
    }

    #endregion

    #region Set Tags

    [HttpPut("{courseId:guid}/tags")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> SetTags(
        [FromRoute] Guid courseId,
        [FromBody] SetCourseTagsRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Setting tags for course {CourseId}", courseId);

        SetCourseTagsCommand command = mapper.Map<SetCourseTagsCommand>(request) with
        {
            CourseId = courseId
        };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to set tags: {Error}", result.Error);
            return result.ToApiResponse("خطا در تنظیم تگ‌های دوره");
        }

        logger.LogInformation("Tags set for course {CourseId}", courseId);
        return result.ToApiResponse("تگ‌های دوره با موفقیت ثبت شدند");
    }

    #endregion

    #region Unpublish Course

    [HttpPatch("{courseId:guid}/unpublish")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Unpublish(
        [FromRoute] Guid courseId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Unpublishing course {CourseId}", courseId);

        Result result = await mediator.Send(new UnpublishCourseCommand(courseId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to unpublish course: {Error}", result.Error);
            return result.ToApiResponse("خطا در لغو انتشار دوره");
        }

        logger.LogInformation("Course {CourseId} unpublished", courseId);
        return result.ToApiResponse("انتشار دوره با موفقیت لغو شد");
    }

    #endregion

    #region Update Course Description

    [HttpPut("{courseId:guid}/description")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> UpdateDescription(
        [FromRoute] Guid courseId,
        [FromBody] UpdateCourseDescriptionRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating description for course {CourseId}", courseId);

        UpdateCourseDescriptionCommand command = mapper.Map<UpdateCourseDescriptionCommand>(request) with
        {
            CourseId = courseId
        };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to update description: {Error}", result.Error);
            return result.ToApiResponse("خطا در بروزرسانی توضیحات دوره");
        }

        logger.LogInformation("Description updated for course {CourseId}", courseId);
        return result.ToApiResponse("توضیحات دوره با موفقیت بروزرسانی شد");
    }

    #endregion

    #region Update Course Details

    [HttpPatch("{courseId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> UpdateDetails(
        [FromRoute] Guid courseId,
        [FromBody] UpdateCourseDetailsRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating details for course {CourseId}", courseId);

        UpdateCourseDetailsCommand command = mapper.Map<UpdateCourseDetailsCommand>(request) with
        {
            CourseId = courseId
        };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to update details: {Error}", result.Error);
            return result.ToApiResponse("خطا در بروزرسانی اطلاعات دوره");
        }

        logger.LogInformation("Details updated for course {CourseId}", courseId);
        return result.ToApiResponse("اطلاعات دوره با موفقیت بروزرسانی شد");
    }

    #endregion

    #region Update Course Level

    [HttpPatch("{courseId:guid}/level")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> UpdateLevel(
        [FromRoute] Guid courseId,
        [FromBody] ChangeCourseLevelRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating level for course {CourseId}", courseId);

        ChangeCourseLevelCommand command = mapper.Map<ChangeCourseLevelCommand>(request) with
        {
            CourseId = courseId
        };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to update level: {Error}", result.Error);
            return result.ToApiResponse("خطا در بروزرسانی سطح دوره");
        }

        logger.LogInformation("Level updated for course {CourseId}", courseId);
        return result.ToApiResponse("سطح دوره با موفقیت بروزرسانی شد");
    }

    #endregion

    #region Update Episode

    [HttpPatch("{courseId:guid}/sections/{sectionId:guid}/episodes/{episodeId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> UpdateEpisode(
        [FromRoute] Guid courseId,
        [FromRoute] Guid sectionId,
        [FromRoute] Guid episodeId,
        [FromForm] UpdateCourseEpisodeRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating episode {EpisodeId} in section {SectionId}", episodeId, sectionId);

        UpdateCourseEpisodeCommand command = mapper.Map<UpdateCourseEpisodeCommand>(request) with
        {
            CourseId = courseId,
            SectionId = sectionId,
            EpisodeId = episodeId,
            // Wrap video only if provided
            VideoFile = request.VideoFile != null ? new FormFileAdapter(request.VideoFile) : null
        };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to update episode: {Error}", result.Error);
            return result.ToApiResponse("خطا در بروزرسانی اپیزود");
        }

        logger.LogInformation("Episode {EpisodeId} updated", episodeId);
        return result.ToApiResponse("اپیزود با موفقیت بروزرسانی شد");
    }

    #endregion

    #region Update Section Title

    [HttpPatch("{courseId:guid}/sections/{sectionId:guid}/title")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> UpdateSectionTitle(
        [FromRoute] Guid courseId,
        [FromRoute] Guid sectionId,
        [FromBody] UpdateSectionTitleRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating title for section {SectionId}", sectionId);

        UpdateSectionTitleCommand command = mapper.Map<UpdateSectionTitleCommand>(request) with
        {
            CourseId = courseId,
            SectionId = sectionId
        };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to update section title: {Error}", result.Error);
            return result.ToApiResponse("خطا در بروزرسانی عنوان بخش");
        }

        logger.LogInformation("Section {SectionId} title updated", sectionId);
        return result.ToApiResponse("عنوان بخش با موفقیت بروزرسانی شد");
    }

    #endregion

    #region Update Slug

    [HttpPatch("{courseId:guid}/slug")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> UpdateSlug(
        [FromRoute] Guid courseId,
        [FromBody] UpdateCourseSlugRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating slug for course {CourseId}", courseId);

        UpdateCourseSlugCommand command = mapper.Map<UpdateCourseSlugCommand>(request) with
        {
            CourseId = courseId
        };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to update slug: {Error}", result.Error);
            return result.ToApiResponse("خطا در بروزرسانی اسلاگ دوره");
        }

        logger.LogInformation("Slug updated for course {CourseId}", courseId);
        return result.ToApiResponse("اسلاگ دوره با موفقیت بروزرسانی شد");
    }

    #endregion

    #region Update Title

    [HttpPatch("{courseId:guid}/title")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> UpdateTitle(
        [FromRoute] Guid courseId,
        [FromBody] UpdateCourseTitleRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating title for course {CourseId}", courseId);

        UpdateCourseTitleCommand command = mapper.Map<UpdateCourseTitleCommand>(request) with
        {
            CourseId = courseId
        };

        Result result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to update title: {Error}", result.Error);
            return result.ToApiResponse("خطا در بروزرسانی عنوان دوره");
        }

        logger.LogInformation("Title updated for course {CourseId}", courseId);
        return result.ToApiResponse("عنوان دوره با موفقیت بروزرسانی شد");
    }

    #endregion

    #region Get Instructor Courses

    [AllowAnonymous]
    [HttpGet("instructor/{instructorId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SimpleCourseResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<PaginatedResponse<SimpleCourseResponse>>> GetInstructorCourses(
        [FromRoute] Guid instructorId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching courses for instructor {InstructorId}", instructorId);

        GetInstructorCoursesQuery query = new(instructorId, pageNumber, pageSize);
        Result<PaginatedResult<SimpleCourseDto>> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to fetch instructor courses: {Error}", result.Error);
            return result.ToApiResponse
                <PaginatedResponse<SimpleCourseResponse>>(
                    "خطا در دریافت لیست دوره‌های مدرس"
                );
        }

        PaginatedResponse<SimpleCourseResponse> response =
            mapper.Map<PaginatedResponse<SimpleCourseResponse>>(result.Value);

        logger.LogInformation("Instructor courses fetched - Count: {Count}, Page: {Page}/{Total}",
            response.Items.Count, pageNumber, result.Value.TotalPages);

        return result.ToApiResponse(mapper.Map<PaginatedResponse<SimpleCourseResponse>>,
            "لیست دوره‌های مدرس با موفقیت دریافت شد");
    }

    #endregion

    #region List Courses

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SimpleCourseResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<PaginatedResponse<SimpleCourseResponse>>> List(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Listing courses - Page: {Page}, Size: {Size}", pageNumber, pageSize);
        var userId = User.GetCurrentUserId();
        var sessionId = HttpContext.GetSessionId();
        CoursesListQuery query = new(pageNumber, pageSize, userId, sessionId);
        Result<PaginatedResult<SimpleCourseDto>> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to list courses: {Error}", result.Error);
            return result.ToApiResponse
                <PaginatedResponse<SimpleCourseResponse>>(
                    "خطا در دریافت لیست دوره‌ها"
                );
        }

        PaginatedResponse<SimpleCourseResponse> response =
            mapper.Map<PaginatedResponse<SimpleCourseResponse>>(result.Value);

        logger.LogInformation("Courses listed - Count: {Count}, Page: {Page}/{Total}",
            response.Items.Count, pageNumber, result.Value.TotalPages);

        return result.ToApiResponse(mapper.Map<PaginatedResponse<SimpleCourseResponse>>,
            "لیست دوره‌ها با موفقیت دریافت شد");
    }

    #endregion

    #region Search Courses

    [AllowAnonymous]
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SimpleCourseResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<PaginatedResponse<SimpleCourseResponse>>> Search(
        [FromQuery] SearchCoursesParameters parameters,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Searching courses with term: {Term}", parameters.SearchTerm);
        var userId = User.GetCurrentUserId();
        var sessionId = HttpContext.GetSessionId();
        SearchCoursesQuery query = mapper.Map<SearchCoursesQuery>(parameters);
        query = query with { UserId = userId, SessionId = sessionId };
        Result<PaginatedResult<SimpleCourseDto>> result =
            await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to search courses: {Error}", result.Error);
            return result.ToApiResponse
                <PaginatedResponse<SimpleCourseResponse>>("خطا در جستجوی دوره‌ها");
        }

        PaginatedResponse<SimpleCourseResponse> response =
            mapper.Map<PaginatedResponse<SimpleCourseResponse>>(result.Value);

        logger.LogInformation("Search completed - Count: {Count}, Page: {Page}/{Total}",
            response.Items.Count, parameters.PageNumber, result.Value.TotalPages);

        return result.ToApiResponse(mapper.Map<PaginatedResponse<SimpleCourseResponse>>,
            "جستجوی دوره‌ها با موفقیت انجام شد");
    }

    #endregion

    #region Search User Courses (My Courses)

    #region My Enrolled Courses

    [Authorize]
    [HttpGet("my-courses")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<SimpleCourseResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<PaginatedResponse<SimpleCourseResponse>>> MyCourses(
        [FromQuery] SearchCoursesParameters parameters,
        CancellationToken cancellationToken = default)
    {
        var userId = User.GetCurrentUserId();
        if (userId is null)
        {
            logger.LogWarning("MyCourses access denied - User ID not found.");
            return ApiResponse<PaginatedResponse<SimpleCourseResponse>>.Fail(
                message: "شناسه کاربر یافت نشد.", ErrorType.Forbidden);
        }

        logger.LogInformation("Fetching enrolled courses for user {UserId} with search term: {SearchTerm}",
            userId, parameters.SearchTerm ?? "N/A");

        var query = mapper.Map<GetMyCoursesCoursesQuery>(parameters) with { UserId = userId.Value };

        Result<PaginatedResult<SimpleCourseDto>> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to fetch enrolled courses for user {UserId}: {Error}", userId, result.Error);
            return result.ToApiResponse<PaginatedResponse<SimpleCourseResponse>>(
                "خطا در دریافت دوره‌های ثبت‌نام‌شده");
        }

        var response = mapper.Map<PaginatedResponse<SimpleCourseResponse>>(result.Value);

        logger.LogInformation("Successfully fetched {Count} enrolled courses for user {UserId} - Page {Page}/{Total}",
            response.Items.Count, userId, parameters.PageNumber, result.Value.TotalPages);

        return ApiResponse<PaginatedResponse<SimpleCourseResponse>>.Success(
            data: response,
            message: "لیست دوره‌های ثبت‌نام‌شده با موفقیت دریافت شد.");
    }

    #endregion

    #endregion

    #region Get By Id

    [AllowAnonymous]
    [HttpGet("{courseId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<CourseResponse>> GetById(
        [FromRoute] Guid courseId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching course {CourseId}", courseId);
        Guid? userId = User.GetCurrentUserId();
        var sessionId = HttpContext.GetSessionId();
        GetCourseByIdQuery query = new(courseId, userId, sessionId);
        Result<CourseDto> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to fetch course: {Error}", result.Error);
            return result.ToApiResponse<CourseResponse>("خطا در دریافت اطلاعات دوره");
        }

        logger.LogInformation("Course {CourseId} fetched", courseId);

        return result.ToApiResponse(
            mapper.Map<CourseResponse>,
            "اطلاعات دوره با موفقیت دریافت شد"
        );
    }

    #endregion

    #region Get Course Title By Id

    [HttpGet("{courseId:guid}/title")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<string>> GetCourseTitleById(
        [FromRoute] Guid courseId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("در حال دریافت عنوان دوره با شناسه {CourseId}", courseId);

        var query = new GetCourseTitleByIdQuery(courseId);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("دریافت عنوان دوره با شناسه {CourseId} با خطا مواجه شد: {Error}", courseId, result.Error);
            return result.ToApiResponse<string>("خطا در دریافت اطلاعات دوره");
        }

        logger.LogInformation("عنوان دوره با شناسه {CourseId} با موفقیت دریافت شد", courseId);
        return result.ToApiResponse(
            title => title,
            "اطلاعات دوره با موفقیت دریافت شد"
        );
    }

    #endregion

    #region Get By Slug

    [AllowAnonymous]
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<CourseResponse>> GetBySlug(
        [FromRoute] string slug,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching course by slug: {Slug}", slug);
        var userId = User.GetCurrentUserId();
        var sessionId = HttpContext.GetSessionId();
        GetCourseBySlugQuery query = new(slug, userId, sessionId);
        Result<CourseDto> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to fetch course by slug: {Error}", result.Error);
            return result.ToApiResponse
                <CourseResponse>("خطا در دریافت اطلاعات دوره");
        }

        logger.LogInformation("Course fetched by slug: {Slug}", slug);

        return result.ToApiResponse(
            mapper.Map<CourseResponse>,
            "اطلاعات دوره با موفقیت دریافت شد"
        );
    }

    #endregion

    #region Get With Sections

    [AllowAnonymous]
    [HttpGet("{courseId:guid}/sections")]
    [ProducesResponseType(typeof(ApiResponse<CourseResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<CourseResponse>> GetWithSections(
        [FromRoute] Guid courseId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching course {CourseId} with sections", courseId);
        var userId = User.GetCurrentUserId();
        var sessionId = HttpContext.GetSessionId();
        GetCourseWithSectionsQuery query = new(courseId, userId, sessionId);
        Result<CourseDto> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to fetch course with sections: {Error}", result.Error);
            return result.ToApiResponse<CourseResponse>("خطا در دریافت اطلاعات دوره");
        }

        logger.LogInformation("Course {CourseId} with sections fetched", courseId);

        return result.ToApiResponse(
            mapper.Map<CourseResponse>,
            "اطلاعات دوره به همراه بخش‌ها با موفقیت دریافت شد"
        );
    }

    #endregion
}
