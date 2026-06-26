using Microsoft.Extensions.Logging;
using Vitastic.App.Common.Abstractions.Files;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Courses.Commands.UpdateSectionEpisode
{
    public sealed class UpdateCourseEpisodeCommandHandler(
        ICourseRepository courseRepository,
        IFileStorageService fileStorageService,
        ILogger<UpdateCourseEpisodeCommandHandler> logger)
        : ICommandHandler<UpdateCourseEpisodeCommand>
    {
        public async Task<Result> Handle(UpdateCourseEpisodeCommand request, CancellationToken cancellationToken)
        {
            // Validate course ID
            Result<CourseId> courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure) return courseIdResult.Error;

            // Get course with sections and episodes
            Course? course = await courseRepository.GetCourseWithSectionAndEpisodes(courseIdResult.Value, cancellationToken);
            if (course is null) return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");

            // Validate section ID
            Result<SectionId> sectionId = SectionId.CreateFrom(request.SectionId);
            if (sectionId.IsFailure) return sectionId.Error;

            // Validate episode ID
            Result<EpisodeId> episodeId = EpisodeId.CreateFrom(request.EpisodeId);
            if (episodeId.IsFailure) return episodeId.Error;

            string? videoFileName = null;

            // Handle video file upload if provided
            if (request.VideoFile != null)
            {
                Result<string> uploadResult = await UploadVideoFile(request.VideoFile, courseIdResult.Value, sectionId.Value, episodeId.Value);
                if (uploadResult.IsFailure)
                    return uploadResult.Error;

                videoFileName = uploadResult.Value;
            }

            // Update episode
            Result result = course.UpdateEpisode(
                sectionId.Value,
                episodeId.Value,
                request.Title,
                request.Duration,
                request.Price,
                request.Currency,
                videoFileName
            );

            if (result.IsFailure)
                return result.Error;

            // Save changes
            await courseRepository.UpdateAsync(course, cancellationToken);

            logger.LogInformation("اپیزود با موفقیت بروزرسانی شد - دوره: {CourseId}, بخش: {SectionId}, اپیزود: {EpisodeId}",
                courseIdResult.Value, sectionId.Value, episodeId.Value);

            return Result.Success();
        }

        private async Task<Result<string>> UploadVideoFile(
            IFile videoFile,
            CourseId courseId,
            SectionId sectionId,
            EpisodeId episodeId)
        {
            try
            {
                var fileKey = await fileStorageService.UploadFileAsync(videoFile,"Course/Episode",episodeId.Value,FileType.Video);
                return Result.Success(fileKey);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطا در آپلود ویدئو برای اپیزود - دوره: {CourseId}, بخش: {SectionId}, اپیزود: {EpisodeId}",
                    courseId.Value, sectionId.Value, episodeId.Value);

                return Error.Failure("Video.UploadFailed", "آپلود ویدئو با خطا مواجه شد");
            }
        }
    }
}
