using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Users.Dtos;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Queries.GetById
{
    public sealed class GetUserQueryHandler(
        IUserQueryService userRepository,
        IRoleRepository roleRepository)
        : IQueryHandler<GetUserQuery, UserDetailDto>
    {
        public async Task<Result<UserDetailDto>> Handle(
            GetUserQuery query,
            CancellationToken cancellationToken)
        {

            var userIdResult = UserId.CreateFrom(query.UserId);
            if (userIdResult.IsFailure)
                return userIdResult.Error;
            return await userRepository.GetByIdAsync(userIdResult.Value, cancellationToken);

        }
    }
}
