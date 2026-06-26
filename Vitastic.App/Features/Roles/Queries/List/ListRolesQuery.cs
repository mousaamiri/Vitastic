using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Roles.Dtos;

namespace Vitastic.App.Features.Roles.Queries.List;

public record ListRolesQuery(string? SearchTerm,int PageNumber,int PageSize)
    : IQuery<PaginatedResult<RoleDto>>;
