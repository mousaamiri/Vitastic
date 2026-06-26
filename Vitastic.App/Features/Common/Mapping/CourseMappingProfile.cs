using AutoMapper;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.App.Features.Instructors.Dtos;
using Vitastic.Domain.Entities.Courses;
using Vitastic.Domain.Entities.Instructors;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Common.Mapping;

public sealed class CourseMappingProfile : Profile
{
    public CourseMappingProfile()
    {
        // Course mappings
        CreateMap<Course, CourseDto>()
            .ForMember(dest => dest.TotalSections, opt
                => opt.MapFrom(src => src.GetTotalSections()))
            .ForMember(dest => dest.TotalEpisodes, opt
                => opt.MapFrom(src => src.GetTotalEpisodes()))
            .ForMember(dest => dest.TotalDuration, opt
                => opt.MapFrom(src => src.GetTotalDuration()))
            .ForMember(dest => dest.FreeEpisodesCount, opt
                => opt.MapFrom(src => src.GetFreeEpisodesCount()))
            .ForMember(dest => dest.HasFreeContent, opt
                => opt.MapFrom(src => src.HasFreeContent()))
            .ForMember(dest => dest.TagIds, opt
                => opt.MapFrom(src => src.Tags.Select(t => t.TagId).ToList()))
            .ForMember(dest => dest.CategoryIds,
                opt
                    => opt.MapFrom(src => src.Categories.Select(c => c.CategoryId).ToList()))
            .ForMember(dest => dest.Sections, opt
                => opt.MapFrom(src => src.Sections))
            .ForMember(dest => dest.ImageName, opt
                => opt.MapFrom<CourseImageUrlResolver>())
            .ForMember(dest => dest.ThumbnailName,
                opt
                    => opt.MapFrom<CourseThumbnailUrlResolver>())
            .ForMember(dest => dest.DemoVideoName,
                opt
                    => opt.MapFrom<CourseVideoUrlResolver>())
            ;
        //Simple dto
        CreateMap<Course, SimpleCourseDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value))
            .ForMember(d => d.Title, opt => opt.MapFrom(s => s.Title.Value))
            .ForMember(d => d.ShortDescription, opt => opt.MapFrom(s => s.ShortDescription.Value))
            .ForMember(d => d.Slug, opt => opt.MapFrom(s => s.Slug.Value))
            .ForMember(dest => dest.ImageName,
                opt
                    => opt.MapFrom<CourseImageUrlResolver>())
            .ForMember(dest => dest.ThumbnailName,
                opt
                    => opt.MapFrom<CourseThumbnailUrlResolver>())
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Price.Value))
            .ForMember(d => d.Currency, opt => opt.MapFrom(s => s.Price.Currency.Value))
            .ForMember(d => d.Level, opt => opt.MapFrom(s => s.Level.ToString()))
            .ForMember(d => d.TotalDuration, opt => opt.MapFrom(s => s.GetTotalDuration()))
            .ForMember(d => d.FreeEpisodesCount, opt => opt.MapFrom(s => s.GetFreeEpisodesCount()))
            .ForMember(d => d.TotalRatings, opt => opt.MapFrom(s => s.TotalRatings))
            .ForMember(
                dest => dest.AverageRating,
                opt => opt.MapFrom(src =>
                    src.Ratings.Count > 0
                        ? Math.Round(
                            src.Ratings.Average(r => r.Rating.Value), 1)
                        : 0m
                )
            );

        CreateMap<Instructor, CourseInstructorDto>()
            //.ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id.Value))
            .ForMember(d => d.Name,
                opt => opt.MapFrom(s => s.FullName))
            .ForMember(d => d.Experties, opt => opt.MapFrom(s => s.Expertise.Value))
            .ForMember(dest => dest.Avatar,
                opt
                    => opt.MapFrom<CourseInstructorAvatarUrlResolver>())
            .ForMember(d => d.TotalRatings, opt => opt.MapFrom(s => s.TotalRatings))
            .ForMember(
                dest => dest.AverageRating,
                opt => opt.MapFrom(src =>
                    src.Ratings.Count > 0
                        ? Math.Round(
                            src.Ratings.Average(r => r.Rating.Value), 1)
                        : 0m
                )
            );

        // Section mappings
        CreateMap<Section, SectionDto>()
            .ForMember(dest => dest.EpisodeCount, opt => opt.MapFrom(src => src.Episodes.Count))
            .ForMember(dest => dest.TotalDuration, opt => opt.MapFrom(src => src.GetTotalDuration()))
            .ForMember(dest => dest.Episodes, opt => opt.MapFrom(src => src.Episodes));

        // Episode mappings
        CreateMap<Episode, EpisodeDto>();
    }
}
#region Resolver

public class CourseImageUrlResolver
    : IValueResolver<Course, CourseDto, string>,
        IValueResolver<Course, SimpleCourseDto, string>
{
    private readonly IFileUrlService _fileUrlService;

    public CourseImageUrlResolver(IFileUrlService fileUrlService)
    {
        _fileUrlService = fileUrlService;
    }

    public string Resolve(
        Course source,
        CourseDto destination,
        string destMember,
        ResolutionContext context)
    {
        return _fileUrlService.GetFileUrl(
            nameof(Course),
            source.Id,
            FileType.Image,
            source.ImageName);
    }

    public string Resolve(
        Course source,
        SimpleCourseDto destination,
        string destMember,
        ResolutionContext context)
    {
        return _fileUrlService.GetFileUrl(
            nameof(Course),
            source.Id,
            FileType.Image,
            source.ImageName);
    }
}
public class CourseThumbnailUrlResolver
    : IValueResolver<Course, CourseDto, string>,
        IValueResolver<Course, SimpleCourseDto, string>
{
    private readonly IFileUrlService _fileUrlService;

    public CourseThumbnailUrlResolver(IFileUrlService fileUrlService)
    {
        _fileUrlService = fileUrlService;
    }

    public string Resolve(
        Course source,
        CourseDto destination,
        string destMember,
        ResolutionContext context)
    {
        return _fileUrlService.GetFileUrl(
            nameof(Course),
            source.Id,
            FileType.Thumbnail,
            source.ThumbnailName);
    }

    public string Resolve(
        Course source,
        SimpleCourseDto destination,
        string destMember,
        ResolutionContext context)
    {
        return _fileUrlService.GetFileUrl(
            nameof(Course),
            source.Id,
            FileType.Thumbnail,
            source.ThumbnailName);
    }
}
public class CourseVideoUrlResolver
    : IValueResolver<Course, CourseDto, string>,
        IValueResolver<Course, SimpleCourseDto, string>
{
    private readonly IFileUrlService _fileUrlService;

    public CourseVideoUrlResolver(IFileUrlService fileUrlService)
    {
        _fileUrlService = fileUrlService;
    }

    public string Resolve(
        Course source,
        CourseDto destination,
        string destMember,
        ResolutionContext context)
    {
        return source.DemoVideoName is not null?  _fileUrlService.GetFileUrl(
            nameof(Course),
            source.Id,
            FileType.Video,
            source.DemoVideoName)
                :null;
    }

    public string Resolve(
        Course source,
        SimpleCourseDto destination,
        string destMember,
        ResolutionContext context)
    {
        return _fileUrlService.GetFileUrl(
            nameof(Course),
            source.Id,
            FileType.Video,
            source.DemoVideoName);
    }
}

public class CourseInstructorAvatarUrlResolver(IFileUrlService fileUrlService)
    :  IValueResolver<Instructor, CourseInstructorDto, string>
{
  
    public string Resolve(Instructor source, CourseInstructorDto destination, string destMember, ResolutionContext context)
     => fileUrlService.GetFileUrl(nameof(User), source.Id.Value, FileType.Image, source.Avatar);

}

#endregion
