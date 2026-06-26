using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Email = Vitastic.Domain.Shared.ValueObjects.Email;

namespace Vitastic.Domain.Shared.Repositories;

public interface IUserRepository:IRepository<User, UserId>
{
    Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
    Task<bool> ExistsByUserNameAsync(UserName userName, CancellationToken cancellationToken=default);
    Task<User?> GetUserByActivationCode(Guid requestActivationCode,CancellationToken cancellationToken=default);
    Task<ActiveCode> GetActivationCodeWithEmail(Email email, CancellationToken cancellationToken=default);
    Task<ActiveCode> GetActivationCodeWithEmailForResetPassword(Email mail, CancellationToken cancellationToken=default);
    Task<User?> GetUserByResetPasswordCodeAsync(ActiveCode activeCode, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken=default);
    Task<User?> GetByUserNameAsync(UserName userName, CancellationToken cancellationToken=default);
    Task<bool> UserIsInstructor(UserId userId, CancellationToken token=default);
    Task<User?> FindUserByActiveCodeAsync(string activationCode, CancellationToken token=default);
    Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken token=default);

    Task ReloadAsync(User user, CancellationToken token=default);
    Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken token=default);
    Task<User?> GetByResetTokenAsync(string resetToken, CancellationToken token=default);
}
