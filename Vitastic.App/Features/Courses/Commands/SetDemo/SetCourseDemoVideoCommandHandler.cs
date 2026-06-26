using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.SetDemo
{
    /// <summary>
    /// Handler for setting a demo video for a course
    /// </summary>
    public sealed class SetCourseDemoVideoCommandHandler(
        ICourseRepository courseRepository,
        IFileStorageService fileStorageService,
        ILogger<SetCourseDemoVideoCommandHandler> logger)
        : ICommandHandler<SetCourseDemoVideoCommand>
    {
        public async Task<Result> Handle(SetCourseDemoVideoCommand request, CancellationToken cancellationToken)
        {
            // Validate course ID
            Result<CourseId> courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure) return courseIdResult.Error;

            // Get the course
            Course? course = await courseRepository.FindAsync(courseIdResult.Value, cancellationToken);
            if (course is null) return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            // Validate video file
            if (request.VideoFile.Length == 0) return Error.Validation("VideoFile.Empty", "فایل ویدئو نمی‌تواند خالی باشد");

            try
            {
                // Upload video to storage
                var courseVideoName = await fileStorageService.UploadFileAsync(
                    request.VideoFile,nameof(Course),courseIdResult.Value,FileType.Video);

                // Create video name value object
                Result<CourseVideoName> videoNameResult = CourseVideoName.Create(courseVideoName);
                if (videoNameResult.IsFailure)
                    return videoNameResult.Error;

                // Set demo video in course aggregate
                Result result = course.SetDemoVideo(videoNameResult.Value);
                if (result.IsFailure)
                    return result.Error;

                // Update course in repository
                await courseRepository.UpdateAsync(course, cancellationToken);

                logger.LogInformation("Demo video set successfully for course {CourseId}", courseIdResult.Value);
                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to set demo video for course {CourseId}", courseIdResult.Value);
                return Error.Failure("DemoVideo.SetFailed", "تنظیم ویدئوی دمو با خطا مواجه شد");
            }
        }
    }
}
