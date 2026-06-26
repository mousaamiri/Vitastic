using AutoMapper;
using Vitastic.Api.Features.Instructors.Requests;
using Vitastic.Api.Features.Instructors.Responses;
using Vitastic.App.Features.Instructors.Commands.Create;
using Vitastic.App.Features.Instructors.Commands.Update;
using Vitastic.App.Features.Instructors.Dtos;

namespace Vitastic.Api.Features.Instructors;

public class InstructorMappingProfile:Profile
{
    public InstructorMappingProfile()
    {
        CreateMap<CreateInstructorRequest, CreateInstructorCommand>();
        CreateMap<UpdateInstructorRequest, UpdateInstructorCommand>();

        CreateMap<InstructorDetailDto,InstructorDetailResponse>();
        CreateMap<InstructorDto,InstructorResponse>();
        CreateMap<InstructorStatisticsDto,InstructorStatisticsResponse>();
    }
}
