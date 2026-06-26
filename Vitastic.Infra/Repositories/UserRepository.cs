using Microsoft.EntityFrameworkCore;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Repositories;

internal class UserRepository(ApplicationWriteDbContext context)
    :BaseRepository<User, UserId>(context), IUserRepository
{

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
        => await ExecuteAsync(async () => await context.Users
            .Include(u=>u.UserRoles)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken), cancellationToken);

    public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        => await ExecuteAsync(async () => await context.Users
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken), cancellationToken);

    public async Task<User?> GetByUserNameAsync(UserName userName, CancellationToken cancellationToken = default)
        => await ExecuteAsync(
            () => context.Users.FirstOrDefaultAsync(x => x.UserName.Equals(userName), cancellationToken), cancellationToken);


    public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await ExecuteAsync(async () => await context.Users.AnyAsync(x
            => x.Email.Equals(email), cancellationToken), cancellationToken);
    }

    public async Task<bool> ExistsByUserNameAsync(UserName userName, CancellationToken cancellationToken = default)
        => await ExecuteAsync(async () => await context.Users
            .AnyAsync(x => x.UserName.Equals(userName),
                cancellationToken), cancellationToken);

    public async Task<User?> GetUserByActivationCode(Guid requestActivationCode, CancellationToken cancellationToken = default)
        => await ExecuteAsync(async () =>
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.ActiveCode.Code.Equals(requestActivationCode), cancellationToken);
            if(user is null)
                throw new InvalidOperationException($"کاربری بااین اکتیویشن کد یافت نشد.: {requestActivationCode}");
            return user;
        },  cancellationToken);



    public async Task<ActiveCode> GetActivationCodeWithEmailForResetPassword(Email mail, CancellationToken cancellationToken=default)
    {
        return await ExecuteAsync(async () =>
        {
            var activationCode = await context.Users
                .Where(x => x.Email.Value == mail.Value && x.IsActive == false)
                .Select(x => x.ActiveCode)
                .FirstOrDefaultAsync(cancellationToken);
            if(activationCode is null)
                throw new InvalidOperationException($"هیچ اکتیویشن کدی برای کاربری بااین ایمیل یافت نشد.: {mail.Value}");
            return activationCode;
        }, cancellationToken);
    }

    public async Task<User?> GetUserByResetPasswordCodeAsync(ActiveCode activeCode,
        CancellationToken cancellationToken = default)
        => await ExecuteAsync(() => context.Users
            .FirstOrDefaultAsync(x => x.IsActive && x.ActiveCode == activeCode, cancellationToken), cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken=default)
        => await ExecuteAsync(() => context.Users.FirstOrDefaultAsync(x => x.Email.Value == email, cancellationToken), cancellationToken);

    public async Task<bool> UserIsInstructor(UserId userId, CancellationToken token = default)
        => await ExecuteAsync(
            () => context.Instructors.AnyAsync(x => x.UserId.Equals(userId), token), token);

    public async Task<User?> FindUserByActiveCodeAsync(string activationCode, CancellationToken token = default)
        => await ExecuteAsync(
            () => context.Users.FirstOrDefaultAsync(x => x.ActiveCode != null
                                                         && x.ActiveCode.Code == activationCode, token), token);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct)
        => await ExecuteAsync(async ()=>await Context.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(
                u => u.RefreshTokens.Any(rt => rt.Token == refreshToken),ct), ct);

    public async Task ReloadAsync(User user, CancellationToken token = default)
        => await Context.Entry(user).ReloadAsync(token);

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken, CancellationToken token = default)
        => await ExecuteAsync(async ()=>
            await Context.Set<RefreshToken>().AddAsync(refreshToken, token), token);

    public async Task<User?> GetByResetTokenAsync(string resetToken, CancellationToken token = default)
        => await ExecuteAsync(async () =>
        {
            User? user = await context.Users.FirstOrDefaultAsync(x =>
                x.ResetPasswordToken != null && x.ResetPasswordToken.Code.Equals(resetToken), token);
            return user;
        },  token);

    public async Task<User?> GetUserByEmail(string requestEmail, CancellationToken cancellationToken=default)
        => await ExecuteAsync(() => context.Users.FirstOrDefaultAsync(x => x.IsActive && x.Email.Value == requestEmail, cancellationToken), cancellationToken);

    public async Task<ActiveCode> GetActivationCodeWithEmail(Email email, CancellationToken cancellationToken = default)
        => await ExecuteAsync(async () =>
        {
            var activationCode = await context.Users.Where(x => x.Email.Value == email.Value && x.IsActive == false).Select(x => x.ActiveCode).FirstOrDefaultAsync(cancellationToken);
            if(activationCode is null)
                throw new InvalidOperationException($"هیچ اکتیویشن کدی برای کاربری بااین ایمیل یافت نشد.: {email.Value}");
            return activationCode;
        }, cancellationToken);
}
