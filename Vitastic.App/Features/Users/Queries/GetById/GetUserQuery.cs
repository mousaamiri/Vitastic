using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Users.Dtos;

namespace Vitastic.App.Features.Users.Queries.GetById
{

    public sealed record GetUserQuery(
        Guid UserId) : IQuery<UserDetailDto>;
}
