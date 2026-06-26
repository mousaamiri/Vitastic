using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Courses.Commands.AddSectionEpisode
{
    public sealed class AddCourseEpisodeCommandHandler(ICourseRepository courseRepository
        , IMapper mapper, IFileStorageService storageService)
        : ICommandHandler<AddCourseEpisodeCommand, EpisodeDto>
    {
        public async Task<Result<EpisodeDto>> Handle(AddCourseEpisodeCommand request,
            CancellationToken cancellationToken)
        {
            //Id validation
            Result<CourseId> courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure) return courseIdResult.Error;
            Result<SectionId> sectionIdResult = SectionId.CreateFrom(request.SectionId);
            if (sectionIdResult.IsFailure) return sectionIdResult.Error;
            Result<SectionId> sectionId = SectionId.CreateFrom(request.SectionId);
            if (sectionId.IsFailure) return sectionId.Error;
            // Checking section belong to this course
            var sectionBelongToCourse = await courseRepository.SectionIsBelongToCourse(sectionIdResult.Value, courseIdResult.Value, cancellationToken);
            if(!sectionBelongToCourse)return Error.Conflict("Course.SectionNotBelongToThisCourse", "بخش به این دوره تعلق ندارد.");
            // Check episode title
            Result<EpisodeTitle> episodeTitleResult = EpisodeTitle.Create(request.Title);
            if (episodeTitleResult.IsFailure) return episodeTitleResult.Error;
            var sectionTitleIsExist = await courseRepository
                .EpisodeTitleIsExistAsync(courseIdResult.Value, sectionIdResult.Value, episodeTitleResult.Value, cancellationToken);
            if (sectionTitleIsExist)
                return Error.Conflict("Course.EpisodeTitleAlreadyExist",
                    "این عنوان قبلا انتخاب شده، لطفا عنوان دیگری انتخاب کنید.");
            // Get aggregate  root
            Course? course = await courseRepository.GetCourseWithSectionAndEpisodes(courseIdResult.Value, cancellationToken);
            if (course is null) return Error.NotFound("Course.NotFound", "دوره مورد نظر یافت نشد");
            // Create episode ID (For naming the video )
            var fileName = "";
            var episodeId = EpisodeId.New();
            if (request.VideoFile!.OpenReadStream().Length>0 )
            {
                fileName =await storageService.UploadFileAsync(request.VideoFile, "Course/Episode", episodeId, FileType.Video);
            }


            Result<Episode> episodeResult = course.AddEpisode(
                sectionId.Value,
                episodeId,
                episodeTitleResult.Value,
                request.Duration,
                Money.Create(request.Price).Value,
                string.IsNullOrEmpty(fileName.Trim())?null: EpisodeVideoName.Create(fileName).Value
            );

            if (episodeResult.IsFailure)
                return episodeResult.Error;

            return mapper.Map<EpisodeDto>(episodeResult.Value);
        }
    }
}
