using AutoMapper;
using Vitastic.Api.Features.Courses.Requests;
using Vitastic.Api.Features.Courses.Responses;
using Vitastic.Api.Wrapper;
using Vitastic.App.Features.Courses.Commands.AddCategory;
using Vitastic.App.Features.Courses.Commands.AddSection;
using Vitastic.App.Features.Courses.Commands.AddSectionEpisode;
using Vitastic.App.Features.Courses.Commands.AddTag;
using Vitastic.App.Features.Courses.Commands.ChangeInstructor;
using Vitastic.App.Features.Courses.Commands.Create;
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
using Vitastic.App.Features.Courses.Commands.UpdateDescription;
using Vitastic.App.Features.Courses.Commands.UpdateDetails;
using Vitastic.App.Features.Courses.Commands.UpdateLevel;
using Vitastic.App.Features.Courses.Commands.UpdateSectionEpisode;
using Vitastic.App.Features.Courses.Commands.UpdateSectionTitle;
using Vitastic.App.Features.Courses.Commands.UpdateSlug;
using Vitastic.App.Features.Courses.Commands.UpdateTitle;
using Vitastic.App.Features.Courses.Dtos;
using Vitastic.App.Features.Courses.Queries.GetMyCourses;
using Vitastic.App.Features.Courses.Queries.Search;

namespace Vitastic.Api.Features.Courses;

public class CourseMappingProfile:Profile
{
    public CourseMappingProfile()
    {
        // Request --> Command
        CreateMap<CreateCourseRequest, CreateCourseCommand>();
        CreateMap<AddCourseSectionRequest, AddCourseSectionCommand>();
        CreateMap<AddCourseEpisodeRequest , AddCourseEpisodeCommand>();
        CreateMap<AddCourseTagRequest, AddCourseTagCommand>();
        CreateMap<AddCourseCategoryRequest, AddCourseCategoryCommand>();
        CreateMap<ChangeCourseInstructorRequest, ChangeCourseInstructorCommand>();
        CreateMap<RemoveCourseDemoVideoCommand, RemoveCourseDemoVideoCommand>();
        CreateMap<ReorderCourseSectionsRequest, ReorderCourseSectionsCommand>();
        CreateMap<SetCourseCategoriesRequest, SetCourseCategoriesCommand>();
        CreateMap<SetCourseDemoVideoRequest, SetCourseDemoVideoCommand>();
        CreateMap<SetCourseImageRequest, SetCourseImageCommand>();
        CreateMap<SetCourseTagsRequest, SetCourseTagsCommand>();
        CreateMap<UpdateCourseDescriptionRequest, UpdateCourseDescriptionCommand>();
        CreateMap<UpdateCourseDetailsRequest, UpdateCourseDetailsCommand>();
        CreateMap<UpdateCourseEpisodeRequest, UpdateCourseEpisodeCommand>();
        CreateMap<UpdateCourseTitleRequest, UpdateCourseTitleCommand>();
        CreateMap<UpdateSectionTitleRequest, UpdateSectionTitleCommand>();
        CreateMap<UpdateCourseSlugRequest, UpdateCourseSlugCommand>();
        CreateMap<ChangeCourseLevelRequest, ChangeCourseLevelCommand>();
        CreateMap<SearchCoursesParameters, SearchCoursesQuery>();
        CreateMap<SearchCoursesParameters, GetMyCoursesCoursesQuery>();

        // Response --> Dto
        CreateMap<SimpleCourseDto, SimpleCourseResponse>();
        CreateMap<CourseDto, CourseResponse>().ReverseMap();
        CreateMap<CourseInstructorDto, CourseInstructorResponse>().ReverseMap();
        CreateMap<SectionDto, SectionResponse>().ReverseMap();
        CreateMap<EpisodeResponse, EpisodeDto>()
            .ForMember(dest => dest.VideoFile,
                opt
                    => opt.MapFrom(src =>src.VideoFile==null? null:new FormFileAdapter(src.VideoFile)))
            .ReverseMap();
    }
}
