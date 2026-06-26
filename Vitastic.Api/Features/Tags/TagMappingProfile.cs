using AutoMapper;
using Vitastic.Api.Features.Tags.Requests;
using Vitastic.Api.Features.Tags.Responses;
using Vitastic.App.Features.Tags.Commands.Create;
using Vitastic.App.Features.Tags.Commands.Update;
using Vitastic.App.Features.Tags.Dtos;

namespace Vitastic.Api.Features.Tags;

public class TagMappingProfile:Profile
{
    public TagMappingProfile()
    {
        CreateMap<TagDto, TagResponse>();
        CreateMap<CreateTagRequest, CreateTagCommand>();
        CreateMap<UpdateTagNameRequest, UpdateTagCommand>();
    }
}
