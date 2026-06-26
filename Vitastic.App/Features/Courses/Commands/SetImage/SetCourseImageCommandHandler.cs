using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.SetImage
{
    public sealed class SetCourseImageCommandHandler(
        ICourseRepository courseRepository,
        IFileStorageService fileStorageService,
        ILogger<SetCourseImageCommandHandler> logger)
        : ICommandHandler<SetCourseImageCommand>
    {
        public async Task<Result> Handle(SetCourseImageCommand request, CancellationToken cancellationToken)
        {
            // Validate course ID
            Result<CourseId> courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure) return courseIdResult.Error;

            // Get course
            Course? course = await courseRepository.FindAsync(courseIdResult.Value, cancellationToken);
            if (course is null) return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            try
            {
                // Upload image with automatic thumbnail creation
                var (imageName, thumbnailName) = await fileStorageService.UploadFileWithThumbnailAsync
                    (request.ImageFile,nameof(Course),courseIdResult.Value);

                // Create value objects
                Result<CourseImageName> imageNameResult = CourseImageName.Create(imageName);
                if (imageNameResult.IsFailure) return imageNameResult.Error;

                Result<CourseThumbnailName> thumbnailNameResult = CourseThumbnailName.Create(thumbnailName);
                if (thumbnailNameResult.IsFailure) return thumbnailNameResult.Error;

                // Set course image
                Result result = course.SetCourseImage(imageNameResult.Value, thumbnailNameResult.Value);
                if (result.IsFailure) return result.Error;

                // Save changes
                await courseRepository.UpdateAsync(course, cancellationToken);

                logger.LogInformation("تصویر دوره با موفقیت تنظیم شد برای دوره {CourseId}", courseIdResult.Value);
                return Result.Success();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در تنظیم تصویر دوره {CourseId}", courseIdResult.Value);
                return Error.Failure("Image.SetFailed", "تنظیم تصویر دوره با خطا مواجه شد");
            }
        }

    }
}
