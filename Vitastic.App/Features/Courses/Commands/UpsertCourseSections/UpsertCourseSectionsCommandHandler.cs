using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Courses.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Courses.Commands.UpsertCourseSections
{
    public sealed class UpsertCourseSectionsCommandHandler(
        ICourseRepository courseRepository,
        IFileStorageService fileStorageService)
        : ICommandHandler<UpsertCourseSectionsCommand>
    {
        public async Task<Result> Handle(UpsertCourseSectionsCommand request, CancellationToken cancellationToken)
        {
            Result<CourseId> courseIdResult = CourseId.CreateFrom(request.CourseId);
            if (courseIdResult.IsFailure)
                return courseIdResult.Error;
            Course? course = await courseRepository.GetCourseWithSectionAndEpisodes(courseIdResult.Value,cancellationToken);
            if(course is null)
                return Error.NotFound("UpsertCourseSectionsCommandHandler.CourseNotFound","دوره ای با این شناسه یافت نشد.");
            List<Section> sections = [];
            foreach (SectionDto sectionDto in request.Sections)
            {
                Result<SectionTitle> sectionTitleResult = SectionTitle.Create(sectionDto.Title);
                if (sectionTitleResult.IsFailure)
                    return sectionTitleResult.Error;
                Result<Section> sectionResult =
                    Section.Create(courseIdResult.Value, sectionTitleResult.Value, sectionDto.DisplayOrder);
                if (sectionResult.IsFailure)
                    return sectionResult.Error;
                Section section = sectionResult.Value;
                sections.Add(section);
                foreach (EpisodeDto episodeDto in sectionDto.Episodes)
                {
                    var episodeId = EpisodeId.New();
                    Result<EpisodeTitle> episodeTitleResult = EpisodeTitle.Create(episodeDto.Title);
                    if (episodeTitleResult.IsFailure)
                        return episodeTitleResult.Error;
                    Result<Money> episodePriceResult =episodeDto.Price ==0?Money.Zero(): Money.Create(episodeDto.Price);
                    if (episodePriceResult.IsFailure)
                        return episodePriceResult.Error;
                    // Upload video file name
                    EpisodeVideoName videoFileName = null;
                    if (episodeDto.VideoFile is not null && episodeDto.VideoFile.Length > 0)
                    {
                        var fileName = await fileStorageService
                            .UploadFileAsync(episodeDto.VideoFile, "Course/Episode", episodeId.Value,
                                FileType.Video);
                        Result<EpisodeVideoName> videoFileNameResult = EpisodeVideoName.Create(fileName);
                        if (videoFileNameResult.IsFailure)
                            return videoFileNameResult.Error;
                        videoFileName = videoFileNameResult.Value;
                    }

                    Result<Episode> addEpisodeResult = section.AddEpisode(episodeId, episodeTitleResult.Value,
                        episodeDto.Duration,
                        episodePriceResult.Value, videoFileName);
                    if (addEpisodeResult.IsFailure)
                        return addEpisodeResult.Error;
                }
            }
            Result updateResult = course.UpdateSections(sections);
            if (updateResult.IsFailure)
                return updateResult.Error;
            await courseRepository.UpdateAsync(course,cancellationToken);
            return Result.Success();
        }

    }
}
