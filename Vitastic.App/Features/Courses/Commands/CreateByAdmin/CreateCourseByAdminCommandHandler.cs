using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.Domain.Entities.Categories.ValueObjects;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.Enums;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Entities.Instructors.ValueObjects;
using Vitastic.Domain.Entities.Tags.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Courses.Commands.CreateByAdmin
{
    public sealed class CreateCourseByAdminCommandHandler(
        ICourseRepository courseRepository,
        IFileStorageService fileStorageService)
        : ICommandHandler<CreateCourseByAdminCommand, Guid>
    {
        public async Task<Result<Guid>> Handle(
            CreateCourseByAdminCommand request,
            CancellationToken cancellationToken)
        {
            #region Validate and create value objects

            // Validate Title
            var titleResult = CourseTitle.Create(request.Title);
            if (titleResult.IsFailure) return titleResult.Error;

            // Validate Description
            var descriptionResult = Description.Create(request.Description);
            if (descriptionResult.IsFailure) return descriptionResult.Error;

            // Validate ShortDescription
            var shortDescResult = ShortDescription.Create(request.ShortDescription);
            if (shortDescResult.IsFailure) return shortDescResult.Error;

            // Validate Slug
            var slugResult = Slug.Create(request.Slug);
            if (slugResult.IsFailure) return slugResult.Error;

            // Validate InstructorId
            var instructorIdResult = InstructorId.CreateFrom(request.InstructorId);
            if (instructorIdResult.IsFailure)
                return CourseErrors.InvalidInstructor; // "مدرس نامعتبر است."

            var instructorId = instructorIdResult.Value;

            // Validate TagIds
            var tagIds = new List<TagId>();
            if (request.TagIds != null)
            {
                foreach (var tagGuid in request.TagIds)
                {
                    var tagIdResult = TagId.CreateFrom(tagGuid);
                    if (tagIdResult.IsFailure)
                        return CourseErrors.InvalidTag; // "برچسب نامعتبر است."
                    tagIds.Add(tagIdResult.Value);
                }
            }

            // Validate CategoryIds
            var categoryIds = new List<CategoryId>();
            if (request.CategoryIds != null)
            {
                foreach (var catGuid in request.CategoryIds)
                {
                    var catIdResult = CategoryId.CreateFrom(catGuid);
                    if (catIdResult.IsFailure)
                        return CourseErrors.InvalidCategory; // "دسته‌بندی نامعتبر است."
                    categoryIds.Add(catIdResult.Value);
                }
            }

            #endregion

            #region Check for duplicate slug

            if (await courseRepository.ExistsBySlugAsync(slugResult.Value, cancellationToken))
                return CourseErrors.SlugAlreadyExists; // "آدرس دوره قبلاً استفاده شده است."

            #endregion

            #region Create course via factory method

            var courseResult = Course.Create(
                title: request.Title,
                description: request.Description,
                shortDescription: request.ShortDescription,
                slug: request.Slug,
                level: (CourseLevel)request.Level,
                instructorId: instructorId
            );

            if (courseResult.IsFailure)
                return courseResult.Error;

            var course = courseResult.Value;

            #endregion

            #region Apply optional settings

            // Set media

            #region Set Media

            // Optional Media
            CourseImageName? imageName = null;
            CourseThumbnailName? thumbnailName = null;
            if (request.ImageFile is not null)
            {
                (string MainFileName, string ThumbnailFileName) fileUploadResult =
                    await fileStorageService.UploadFileWithThumbnailAsync(request.ImageFile, nameof(Course),
                        course.Id);
                Result<CourseImageName> imgResult = CourseImageName.Create(fileUploadResult.MainFileName);
                if (imgResult.IsFailure) return imgResult.Error;
                imageName = imgResult.Value;

                Result<CourseThumbnailName>
                    thumbResult = CourseThumbnailName.Create(fileUploadResult.ThumbnailFileName);
                if (thumbResult.IsFailure) return thumbResult.Error;
                thumbnailName = thumbResult.Value;
            }
            else
            {
                imageName = CourseImageName.Default();
                thumbnailName = CourseThumbnailName.Default();
            }

            if (request.DemoVideoFile is not null)
            {
                var fileUploadResult =
                    await fileStorageService.UploadFileAsync(request.DemoVideoFile, nameof(Course), course.Id,
                        FileType.Video);
                Result<CourseVideoName> videoResult = CourseVideoName.Create(fileUploadResult);
                if (videoResult.IsFailure) return videoResult.Error;
                CourseVideoName? demoVideoName = videoResult.Value;
                // Set video
                Result setVideoResult = course.SetDemoVideo(demoVideoName);
                if (setVideoResult.IsFailure)
                    return setVideoResult.Error;
            }
            //Set images
            Result setImageResult = course.SetCourseImage(imageName, thumbnailName);
            if (setImageResult.IsFailure)
                return setImageResult.Error;

            #endregion

            // Certificate
            if (request.HasCertificate)
            {
                var certResult = course.EnableCertificate();
                if (certResult.IsFailure)
                    return certResult.Error;
            }

            // Assign tags and categories
            if (request.TagIds is not null)
            {
                Result setTagsResult = course.SetTags(tagIds);
                if (setTagsResult.IsFailure)
                    return setTagsResult.Error;
            }

            if (request.CategoryIds is not null)
            {
                Result setCategoriesResult = course.SetCategories(categoryIds);
                if (setCategoriesResult.IsFailure)
                    return setCategoriesResult.Error;
            }

            #endregion

            #region Save course

            await courseRepository.AddAsync(course, cancellationToken);
            return Result.Success(course.Id.Value);

            #endregion
        }
    }
}
