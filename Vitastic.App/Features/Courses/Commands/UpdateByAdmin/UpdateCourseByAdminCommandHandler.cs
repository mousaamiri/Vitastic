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

namespace Vitastic.App.Features.Courses.Commands.UpdateByAdmin
{
    public sealed class UpdateCourseByAdminCommandHandler
        (ICourseRepository courseRepository,
            IFileStorageService fileStorageService)
        : ICommandHandler<UpdateCourseByAdminCommand>
    {
        public async Task<Result> Handle(UpdateCourseByAdminCommand request, CancellationToken cancellationToken)
        {
            #region Find Course

            var idResult = CourseId.CreateFrom(request.CourseId);
            if (idResult.IsFailure)
                return idResult.Error;

            Course? course = await courseRepository.FindIncludeMembersAsync(idResult.Value, cancellationToken);
            if (course is null)
                return Error.NotFound("UpdateCourseByAdminCommand.CourseNotFound", "دوره یافت نشد.");

            #endregion

            #region Validate and Update Basic Properties

            // Title
            var titleResult = CourseTitle.Create(request.Title);
            if (titleResult.IsFailure) return titleResult.Error;
            if (!course.Title.Equals(titleResult.Value))
            {
                var setTitleResult = course.UpdateTitle(titleResult.Value);
                if (setTitleResult.IsFailure) return setTitleResult.Error;
            }

            // Description
            var descriptionResult = Description.Create(request.Description);
            if (descriptionResult.IsFailure) return descriptionResult.Error;
            var setDescResult = course.UpdateDescription(descriptionResult.Value);
            if (setDescResult.IsFailure) return setDescResult.Error;

            // ShortDescription
            var shortDescResult = ShortDescription.Create(request.ShortDescription);
            if (shortDescResult.IsFailure) return shortDescResult.Error;
            var setShortDescResult = course.UpdateShortDescription(shortDescResult.Value);
            if (setShortDescResult.IsFailure) return setShortDescResult.Error;

            // Slug
            var slugResult = Slug.Create(request.Slug);
            if (slugResult.IsFailure) return slugResult.Error;
            if (!course.Slug.Equals(slugResult.Value))
            {

                if (await courseRepository.ExistsBySlugAsync(slugResult.Value, cancellationToken))
                    return CourseErrors.SlugAlreadyExists;
                var setSlugResult = course.UpdateSlug(slugResult.Value);
                if (setSlugResult.IsFailure) return setSlugResult.Error;
            }

            #endregion

            #region Update Instructor

            var instructorIdResult = InstructorId.CreateFrom(request.InstructorId);
            if (instructorIdResult.IsFailure) return CourseErrors.InvalidInstructor;
            var setMasterResult = course.ChangeInstructor(instructorIdResult.Value);
            if (setMasterResult.IsFailure) return setMasterResult.Error;

            #endregion

            #region Update Tags

            if (request.TagIds != null)
            {
                var tagIds = new List<TagId>();
                foreach (var tagGuid in request.TagIds)
                {
                    var tagIdResult = TagId.CreateFrom(tagGuid);
                    if (tagIdResult.IsFailure)
                        return CourseErrors.InvalidTag;
                    tagIds.Add(tagIdResult.Value);
                }

                var setTagsResult = course.SetTags(tagIds);
                if (setTagsResult.IsFailure) return setTagsResult.Error;
            }

            #endregion

            #region Update course level

            var setLevelResult = course.ChangeLevel((CourseLevel)request.Level);
            if (setLevelResult.IsFailure) return setLevelResult.Error;

            #endregion
            #region Update Categories

            if (request.CategoryIds != null)
            {
                var categoryIds = new List<CategoryId>();
                foreach (var catGuid in request.CategoryIds)
                {
                    var catIdResult = CategoryId.CreateFrom(catGuid);
                    if (catIdResult.IsFailure)
                        return CourseErrors.InvalidCategory;
                    categoryIds.Add(catIdResult.Value);
                }

                var setCategoriesResult = course.SetCategories(categoryIds);
                if (setCategoriesResult.IsFailure) return setCategoriesResult.Error;
            }

            #endregion

            #region Update Media Files

            // Image and Thumbnail

            if (request.ImageFile is not null)
            {
                var fileUploadResult = await fileStorageService.UploadFileWithThumbnailAsync(
                    request.ImageFile, nameof(Course), course.Id);

                var imgResult = CourseImageName.Create(fileUploadResult.MainFileName);
                if (imgResult.IsFailure) return imgResult.Error;
                CourseImageName? imageName = imgResult.Value;

                var thumbResult = CourseThumbnailName.Create(fileUploadResult.ThumbnailFileName);
                if (thumbResult.IsFailure) return thumbResult.Error;
                CourseThumbnailName? thumbnailName = thumbResult.Value;
                //Set images
                var setImageResult = course.SetCourseImage(imageName, thumbnailName);
                if (setImageResult.IsFailure) return setImageResult.Error;
            }



            // Demo Video
            if (request.DemoVideoFile is not null)
            {
                var videoUploadResult = await fileStorageService.UploadFileAsync(
                    request.DemoVideoFile, nameof(Course), course.Id, FileType.Video);

                var videoResult = CourseVideoName.Create(videoUploadResult);
                if (videoResult.IsFailure) return videoResult.Error;

                var setVideoResult = course.SetDemoVideo(videoResult.Value);
                if (setVideoResult.IsFailure) return setVideoResult.Error;
            }

            #endregion

            #region Apply Optional Settings

            if (request.HasCertificate)
            {
                var certResult = course.EnableCertificate();
                if (certResult.IsFailure) return certResult.Error;
            }

            #endregion

            #region Save Changes

            await courseRepository.UpdateAsync(course, cancellationToken);
            return Result.Success(course.Id.Value);

            #endregion
        }
    }
}
