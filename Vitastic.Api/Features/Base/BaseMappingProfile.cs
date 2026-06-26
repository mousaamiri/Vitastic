using AutoMapper;
using Vitastic.App.Features.Common.Dtos;

namespace Vitastic.Api.Features.Base;

public class BaseMappingProfile:Profile
{
    public BaseMappingProfile()
    {
        // Maps PaginatedResult<> → PaginatedResponse<>
        CreateMap(
            typeof(PaginatedResult<>),
            typeof(PaginatedResponse<>));
    }
}
