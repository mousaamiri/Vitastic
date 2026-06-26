using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Npgsql;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Common.Abstractions.Services.Queries;
using Vitastic.App.Features.Users.Dtos;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.ValueObjects;
using Vitastic.Infra.Data;

namespace Vitastic.Infra.Services.Queries;

internal class UserQueryService(
    string connectionString,
    ApplicationWriteDbContext readDbContext,
    IMapper mapper,
    IFileUrlService fileUrlService,
    ILogger<UserQueryService> logger) : IUserQueryService
{
    public async Task<bool> HasRoleAsync(
        UserId userId,
        RoleId roleId,
        CancellationToken cancellationToken)
    {
        try
        {
            return await readDbContext.UserRoles.AnyAsync(x => x.UserId == userId && x.RoleId == roleId,
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error checking user role | UserId: {UserId}, RoleId: {RoleId}",
                userId,
                roleId);
            throw;
        }
    }

    public async Task<UserDetailDto?> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<User> query = readDbContext.Users.AsQueryable();
            User? user = await query
                .AsNoTracking()
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
                return null;
            UserDetailDto? userDto = mapper.Map<UserDetailDto>(user);
            userDto?.RoleNames = await readDbContext.UserRoles
                .Where(u => u.UserId == user.Id)
                .Join(readDbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                .Select(r => r.Name.ToString())
                .ToListAsync(cancellationToken);
            userDto!.WalletBalance = await GetUserWalletBalanceAsync(user.Id, cancellationToken);
            return userDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user by email: {Email}", email);
            throw;
        }
    }

    public async Task<UserDetailDto?> GetByIdAsync(
        UserId userId,
        CancellationToken token)
    {
        try
        {
            IQueryable<User> query = readDbContext.Users.AsQueryable();
            User? user = await query
                .AsNoTracking()
                .Where(x => x.Id == userId)
                .FirstOrDefaultAsync(token);
            UserDetailDto? userDto = mapper.Map<UserDetailDto>(user);
            if (userDto == null)
                return null;
            userDto?.RoleNames = await readDbContext.UserRoles
                .Where(u => u.UserId == user!.Id)
                .Join(readDbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                .Select(r => r.Name.ToString())
                .ToListAsync(token);
            userDto!.WalletBalance = await GetUserWalletBalanceAsync(userId, token);
            userDto!.RoleNames = userDto!.RoleNames.Count != 0 ? userDto.RoleNames : ["کاربر عادی"];
            return userDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user by id: {UserId}", userId);
            throw;
        }
    }

    private async Task<decimal> GetUserWalletBalanceAsync(UserId userId, CancellationToken token)
        => await readDbContext
            .Wallets
            .Where(w => w.UserId == userId)
            .Select(w => w.Balance.Value)
            .FirstOrDefaultAsync(token);


    public async Task<UserDetailDto?> GetByUsernameAsync(
        UserName userName,
        CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<User> query = readDbContext.Users.AsQueryable();
            User? user = await query
                .AsNoTracking()
                .Where(x => x.UserName == userName)
                .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
                return null;
            UserDetailDto? userDto = mapper.Map<UserDetailDto>(user);
            userDto?.RoleNames = await readDbContext.UserRoles
                .Where(u => u.UserId == user.Id)
                .Join(readDbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                .Select(r => r.Name.ToString())
                .ToListAsync(cancellationToken);
            userDto!.WalletBalance = await GetUserWalletBalanceAsync(user.Id, cancellationToken);
            userDto!.RoleNames = userDto!.RoleNames.Count != 0 ? userDto.RoleNames : ["کاربر عادی"];
            return userDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user by username: {UserName}", userName);
            throw;
        }
    }

    public async Task<UserAvatarInfoDto?> GetUserAvatarInfoAsync(UserId id, CancellationToken cancellationToken)
    {
        try
        {
            IQueryable<User> query = readDbContext.Users.AsQueryable();
            User? user = await query
                .AsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync(cancellationToken);
            if (user is null)
                return null;
            UserAvatarInfoDto? userDto = mapper.Map<UserAvatarInfoDto>(user);
            userDto?.RoleNames = await readDbContext.UserRoles
                .Where(u => u.UserId == user.Id)
                .Join(readDbContext.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r)
                .Select(r => r.Name.ToString())
                .ToListAsync(cancellationToken);
            userDto!.RoleNames = userDto!.RoleNames.Count != 0 ? userDto.RoleNames : ["کاربر عادی"];
            return userDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user avatar info : {Id}", id);
            throw;
        }
    }

    public async Task<string?> GetUserAvatarImagePathAsync(UserId id, CancellationToken cancellationToken)
    {
        try
        {
            var userAvatar = await
                readDbContext.Users
                    .AsNoTracking()
                    .Where(x => x.Id == id)
                    .Select(x=>x.UserAvatar.FileName)
                    .FirstOrDefaultAsync(cancellationToken);
            if(userAvatar is null)
                return null;
            return fileUrlService.GetFileUrl(nameof(User),id,FileType.Image,userAvatar);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting user avatar image path  : {Id}", id);
            throw;
        }
    }

    public async Task<(IReadOnlyList<UserDto> items, int total)> SearchAsync(
        string? searchTerm,
        int pageNumber,
        int pageSize,
        CancellationToken token = default)
    {
        try
        {
            IQueryable<User> usersQuery;

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                usersQuery = readDbContext.Users;
            }
            else
            {
                var normalizedSearchTerm = $"%{searchTerm}%";

                usersQuery = readDbContext.Users
                    .FromSqlRaw(@"
                    SELECT *
                    FROM ""Users""
                    WHERE
                        UPPER(""FirstName"") LIKE UPPER(@searchTerm) OR
                        UPPER(""LastName"") LIKE UPPER(@searchTerm) OR
                        UPPER(""Email"") LIKE UPPER(@searchTerm) OR
                        UPPER(""PhoneNumber"") LIKE UPPER(@searchTerm)
                ", new NpgsqlParameter("@searchTerm", normalizedSearchTerm));
            }

            var totalCount = await usersQuery.CountAsync(token);

            List<User> pagedUsers = await usersQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(token);
            List<UserDto>? dtos = mapper.Map<List<UserDto>>(pagedUsers);
            return (dtos, totalCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "خطا در هنگام جستجوی کاربران با عبارت: {SearchTerm}", searchTerm);
            throw;
        }
    }

}
