using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Features.Users.Dtos;

namespace Vitastic.App.Features.Users.Queries.GetByUserName
{
    public sealed record GetUserByUsernameQuery(
        string UserName) : IQuery<UserDetailDto>;
}
