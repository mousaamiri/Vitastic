using AutoMapper;
using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Data;
using Vitastic.Domain.Entities.Cart;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.Login;

public sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    ICartRepository cartRepository,
    ICartIdentityService cartIdentityService,
    IJwTokenService jwtService,
    IRoleRepository roleRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper)
    : ICommandHandler<LoginUserCommand, JwtResult>
{
    #region Helper Methods

    // Check if identifier is email (contains @) or username
    private static bool IsEmail(string identifier) =>
        identifier.Contains('@');

    #endregion

    public async Task<Result<JwtResult>> Handle(
        LoginUserCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            #region Find User

            User? user;

            if (IsEmail(command.Identifier))
            {
                Result<Email> emailResult = Email.Create(command.Identifier);
                if (emailResult.IsFailure)
                    return Error.Validation(
                        "Login.InvalidEmail",
                        "فرمت ایمیل واردشده معتبر نیست.");

                user = await userRepository.GetByEmailAsync(emailResult.Value, cancellationToken);
            }
            else
            {
                Result<UserName> userNameResult = UserName.Create(command.Identifier);
                if (userNameResult.IsFailure)
                    return Error.Validation(
                        "Login.InvalidUserName",
                        "فرمت نام کاربری واردشده معتبر نیست.");

                user = await userRepository.GetByUserNameAsync(
                    userNameResult.Value, cancellationToken);
            }

            if (user is null)
                return Error.NotFound(
                    "Login.InvalidCredentials",
                    "نام کاربری/ایمیل یا رمز عبور اشتباه است.");

            #endregion

            #region GetUser Role

            var roles =( await roleRepository.GetUserRolesAsync(user.Id, cancellationToken))
                .Select(r=>r.Name.Value)
                .ToArray();

            #endregion
            #region Verify Password

            Result<bool> isPasswordValidResult = user.VerifyPassword(command.Password);
            if (isPasswordValidResult.IsFailure)
                return isPasswordValidResult.Error;

            if (!isPasswordValidResult.Value)
                return Error.NotFound(
                    "Login.InvalidCredentials",
                    "نام کاربری/ایمیل یا رمز عبور اشتباه است.");

            #endregion

            #region Merge Guest Cart (if exists)

            var identity = cartIdentityService.GetCartIdentity();

            if (identity.IsGuest && !string.IsNullOrEmpty(identity.SessionId))
            {
                var guestCart = await cartRepository
                    .GetBySessionIdAsync(identity.SessionId, cancellationToken);

                if (guestCart is not null && !guestCart.IsEmpty())
                {
                    // Get or create user cart
                    Cart? userCart = await cartRepository
                        .GetByUserIdAsync(user.Id, cancellationToken);

                    if (userCart is null)
                    {
                        var createResult = Cart.Create(user.Id, user.UserFullName, user.Email);
                        if (createResult.IsFailure)
                            return createResult.Error;

                        userCart = createResult.Value;
                        await cartRepository.AddAsync(userCart, cancellationToken);
                        await unitOfWork.SaveChangesAsync(cancellationToken);

                    }

                    // Merge guest cart into user cart
                    var mergeResult = userCart.MergeGuestItems(guestCart.Id, guestCart.Items);
                    if (mergeResult.IsFailure)
                        return mergeResult.Error;

                    await cartRepository.UpdateAsync(userCart, cancellationToken);
                    await cartRepository.DeleteAsync(guestCart, cancellationToken);
                }
            }

            #endregion

            #region Generate JWT Token

            JwtResult token = jwtService.GenerateAccessToken(user,roles);

            #endregion

            #region Save Refresh Token

            var refreshToken = Domain.Entities.Users.RefreshToken.Create(
                userId: user.Id,
                token: token.RefreshToken,
                lifetime: TimeSpan.FromDays(2));

            await userRepository.AddRefreshTokenAsync(
                refreshToken, cancellationToken);

            #endregion



            return token;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}
