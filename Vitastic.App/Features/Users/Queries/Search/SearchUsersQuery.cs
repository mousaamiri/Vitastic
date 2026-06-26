using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Users.Dtos;

namespace Vitastic.App.Features.Users.Queries.Search;

public record SearchUsersQuery(
    string? SearchTerm,
    int PageNumber=1,
    int PageSize=10):IQuery<PaginatedResult<UserDto>>;
