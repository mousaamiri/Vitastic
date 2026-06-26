using AutoMapper;
using Vitastic.App.Features.Tags.Dtos;
using Vitastic.Domain.Entities.Tags;

namespace Vitastic.App.Features.Common.Mapping;

public class TagMappingProfile:Profile
{
    public TagMappingProfile()
    {
        // Tag → TagDto
        CreateMap<Tag, TagDto>();
    }
}
