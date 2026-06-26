using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Users.Dtos;

namespace Vitastic.App.Features.Users.Queries.GetByEmail
{
    public sealed record GetUserByEmailQuery(
        string Email) : IQuery<UserDetailDto>;
}
