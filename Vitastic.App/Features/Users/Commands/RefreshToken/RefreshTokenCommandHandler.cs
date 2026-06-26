using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Data;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.App.Features.Users.Commands.RefreshToken
{
    public sealed class RefreshTokenCommandHandler(
        IUserRepository userRepository,
        IJwTokenService jwtService,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<RefreshTokenCommand, JwtResult>
    {
        public async Task<Result<JwtResult>> Handle(
            RefreshTokenCommand command,
            CancellationToken cancellationToken)
        {
            // ──── 1. Find user with  Refresh Token ────
            var user = await userRepository.GetByRefreshTokenAsync(
                command.RefreshToken, cancellationToken);

            if (user is null)
                return Error.NotFound(
                    "RefreshToken.Invalid",
                    "توکن نامعتبر است.");
            #region GetUser Role

            var roles =( await roleRepository.GetUserRolesAsync(user.Id, cancellationToken))
                .Select(r=>r.Name.Value)
                .ToArray();

            #endregion
            // ──── 2.Find Refresh Token ────
            var existingToken = user.RefreshTokens
                .Single(rt => string.Equals(rt.Token, command.RefreshToken, StringComparison.Ordinal));

            // ──── 3 . if it revoked -> the probability of robbery ! ────
            if (existingToken.IsRevoked)
            {
                // DeActive all active tokens (security)
                user.RevokeAllRefreshTokens();
                await unitOfWork.SaveChangesAsync(cancellationToken);

                return Error.Forbidden(
                    "RefreshToken.PossibleTheft",
                    "نشست شما به دلایل امنیتی باطل شد. لطفاً دوباره وارد شوید.");
            }

            // ────4. if it has expired ────
            if (existingToken.IsExpired)
                return Error.Validation(
                    "RefreshToken.Expired",
                    "توکن منقضی شده است. لطفاً دوباره وارد شوید.");

            // ──── 5. generating new Token ────
            JwtResult newJwt = jwtService.GenerateAccessToken(user,roles);

            // ──── 6. Rotate : save the new + cancel old ────
            existingToken.Revoke(replacedByToken: newJwt.RefreshToken);

            var newRefreshToken = Domain.Entities.Users.RefreshToken.Create(
                userId: user.Id,
                token: newJwt.RefreshToken,
                lifetime: TimeSpan.FromDays(7));

            user.AddRefreshToken(newRefreshToken);

            //✅ Changes are saved in the Behavior Unit of Work. ✅
            return newJwt;
        }
    }
}
