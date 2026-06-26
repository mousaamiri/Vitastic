#region Using

using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users.Events;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Models;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

#endregion

namespace Vitastic.Domain.Entities.Users;

/// <summary>
/// User aggregate root
/// TKey = UserId (strongly-typed ID)
/// </summary>
public class User : AggregateRoot<UserId>
{
    #region Properties

    public Email Email { get; private set; }

    public UserName UserName { get; private set; }

    public PhoneNumber? PhoneNumber { get; private set; }

    public FirstName? FirstName { get; private set; }

    public LastName? LastName { get; private set; }

    // Computed once and cached — invalidated on name change
    public FullName? UserFullName => BuildFullName();

    public Password Password { get; private set; }

    /// <summary>
    /// Activation code for email verification.
    /// Used when: User Registration, Email Change.
    /// Null when: User activated or created by admin with IsActive=true.
    /// </summary>
    public ActiveCode? ActiveCode { get; private set; }

    /// <summary>
    /// Reset password token for password recovery.
    /// Used when: User requests password reset.
    /// Null when: Not requested or already used.
    /// </summary>
    public ActiveCode? ResetPasswordToken { get; private set; }

    public UserAvatar UserAvatar { get; private set; }

    /// <summary>
    /// Changes every time user sensitive data changes.
    /// Used for: Force logout / session invalidation.
    /// </summary>
    public SecurityStamp SecurityStamp { get; private set; }

    #endregion

    #region Primitive Properties

    public bool IsActive { get; private set; }

    public DateTimeOffset RegisterDate { get; private set; }

    public DateTimeOffset? LastLoginDate { get; private set; }

    #endregion

    #region Collections

    private readonly HashSet<UserRole> _userRoles = [];
    public IReadOnlyCollection<UserRole> UserRoles => _userRoles;

    private readonly List<RefreshToken> _refreshTokens = [];
    public IReadOnlyCollection<RefreshToken> RefreshTokens => _refreshTokens.AsReadOnly();

    #endregion

    #region Constructors

    protected User() { } // EF Core

    private User(
        UserName userName,
        Email email,
        Password password) : base(UserId.New())
    {
        UserName = userName;
        Email = email;
        Password = password;
        RegisterDate = DateTimeOffset.UtcNow;
        UserAvatar = UserAvatar.Default();
        IsActive = false;
        ActiveCode = null;
        UpdateSecurityStamp();

    }

    private User(UserName userName, Email email, Password password,
        FirstName firstName, LastName lastName, PhoneNumber? phoneNumber, UserAvatar? avatar, bool isActive)
        : base(UserId.New())
    {
        UserName = userName;
        Email = email;
        Password = password;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        RegisterDate = DateTimeOffset.UtcNow;
        UserAvatar = avatar?? UserAvatar.Default();
        IsActive = isActive;
        ActiveCode = null;
        UpdateSecurityStamp();
    }

    #endregion

    #region Factory Methods

    /// <summary>
    /// Factory method for user self-registration.
    /// Sets IsActive = false, generates ActiveCode.
    /// User must activate account via email verification.
    /// </summary>
    public static Result<User> Register(
        UserName userName,
        Email email,
        Password password,
        string callbackUrl)
    {
        var user = new User(userName, email, password)
        {
            ActiveCode = ActiveCode.Generate()
        };

        user.RaiseDomainEvent( UserRegisteredDomainEvent.Create(
            user.Id,
            userName,
            email,
            user.ActiveCode,
            callbackUrl));

        return Result.Success(user);
    }

    /// <summary>
    /// Factory method for admin user creation.
    /// Creates user with full data and activates immediately.
    /// No email verification required.
    /// </summary>
    public static Result<User> CreateByAdmin(UserName userName, Email email, Password password,
        FirstName firstName, LastName lastName, PhoneNumber? phoneNumber, UserAvatar? avatar, bool isActive)
    {
        var create = new User(userName, email, password, firstName, lastName,phoneNumber, avatar, isActive);
        return Result.Success(create);
    }
    #endregion

    #region Profile Management

    public Result UpdateProfile(
        FirstName? firstName,
        LastName? lastName,
        PhoneNumber? phoneNumber)
    {
        // Track if name changed for FullName rebuild
        bool nameChanged = false;
        if(firstName is not null){FirstName = firstName; nameChanged = true;}
        if (lastName is not null) {LastName = lastName; nameChanged = true; }
        if(phoneNumber is not null)PhoneNumber = phoneNumber;
        if (nameChanged)
            BuildFullName();

        RaiseDomainEvent( UserUpdatedDomainEvent.Create(Id));

        return Result.Success();
    }

    public Result ChangeAvatar(UserAvatar newAvatar)
    {
        if (newAvatar is null)
            return UserErrors.InvalidAvatar;

        UserAvatar oldAvatar = UserAvatar;
        UserAvatar = newAvatar;

        RaiseDomainEvent( UserAvatarChangedDomainEvent.Create(
            Id,
            oldAvatar,
            newAvatar));

        return Result.Success();
    }

    #endregion

    #region Email Management

    public Result ChangeEmail(string newEmail)
    {
        Result<Email> emailResult = Email.Create(newEmail);

        if (emailResult.IsFailure)
            return emailResult.Error;

        if (Email.Equals(emailResult.Value))
            return Result.Success();

        Email oldEmail = Email;
        Email = emailResult.Value;

        // Requires re-activation with new email
        IsActive = false;
        ActiveCode = ActiveCode.Generate();
        UpdateSecurityStamp();

        RaiseDomainEvent( UserEmailChangedDomainEvent.Create(
            Id,
            oldEmail,
            Email,
            ActiveCode));

        return Result.Success();
    }

    #endregion

    #region Password Management

    public Result ChangePassword(string currentPassword, string newPassword)
    {
        if (!Password.Verify(currentPassword))
            return UserErrors.InvalidPassword;

        Result<Password> passwordResult = Password.Create(newPassword);

        if (passwordResult.IsFailure)
            return passwordResult.Error;

        Password = passwordResult.Value;
        UpdateSecurityStamp();

        RaiseDomainEvent( UserPasswordChangedDomainEvent.Create(Id));

        return Result.Success();
    }

    /// <summary>
    /// Request password reset — generates reset token.
    /// Used when: User forgets password.
    /// </summary>
    public Result RequestPasswordReset(string callbackUrl)
    {
        ResetPasswordToken = ActiveCode.Generate();

        RaiseDomainEvent( PasswordResetRequestedDomainEvent.Create(
            Id,
            UserName,
            Email,
            ResetPasswordToken,
            callbackUrl));

        return Result.Success();
    }

    /// <summary>
    /// Reset password using reset token.
    /// Validates token, checks expiry, and updates password.
    /// </summary>
    public Result ResetPasswordWithToken(string token, string newPassword)
    {
        if (ResetPasswordToken is null)
            return UserErrors.NoResetTokenAvailable;

        if (!string.Equals(ResetPasswordToken.Code, token, StringComparison.Ordinal))
            return UserErrors.InvalidResetToken;

        if (ResetPasswordToken.IsExpired())
            return UserErrors.ResetTokenExpired;

        Result<Password> passwordResult = Password.Create(newPassword);

        if (passwordResult.IsFailure)
            return passwordResult.Error;

        Password = passwordResult.Value;
        ResetPasswordToken = null;
        UpdateSecurityStamp();

        RaiseDomainEvent( UserPasswordResetDomainEvent.Create(Id));

        return Result.Success();
    }

    #endregion

    #region Activation

    /// <summary>
    /// Activate user account with activation code.
    /// Validates the provided code against the stored ActiveCode.
    /// After activation: IsActive = true, ActiveCode = null.
    /// </summary>
    public Result Activate(string code)
    {
        if (IsActive)
            return UserErrors.UserIsActive;

        if (ActiveCode is null)
            return UserErrors.NoActivationCodeAvailable;

        // Validate the provided code matches
        if (!string.Equals(ActiveCode.Code, code, StringComparison.Ordinal))
            return UserErrors.InvalidActivationCode;

        if (ActiveCode.IsExpired())
            return UserErrors.ActiveCodeIsExpired;

        IsActive = true;
        ActiveCode = null;
        UpdateSecurityStamp();

        RaiseDomainEvent( UserActivatedDomainEvent.Create(Id));

        return Result.Success();
    }

    public Result Deactivate()
    {
        if (!IsActive)
            return UserErrors.AlreadyInactive;

        IsActive = false;
        UpdateSecurityStamp();

        RaiseDomainEvent( UserDeactivatedDomainEvent.Create(Id));

        return Result.Success();
    }
    public Result UpdateUserName(UserName userName)
    {
        UserName = userName;
        UpdateSecurityStamp();

        return Result.Success();
    }

    /// <summary>
    /// Regenerate activation code for pending activation.
    /// Used when: User didn't receive code or code expired.
    /// Only works when user is NOT active.
    /// </summary>
    public Result RegenerateActivationCode(string callbackUrl)
    {
        if (IsActive)
            return UserErrors.AlreadyActive;

        ActiveCode = ActiveCode.Generate();

        RaiseDomainEvent( ActivationCodeRegeneratedDomainEvent.Create(
            Id,
            UserName,
            Email,
            ActiveCode,
            callbackUrl));

        return Result.Success();
    }

    #endregion

    #region Authentication

    public Result<bool> VerifyPassword(string password)
    {
        if (!IsActive)
            return UserErrors.AccountNotActivated;

        bool isValid = Password.Verify(password);

        if (isValid)
        {
            UpdateLastLogin();
            RaiseDomainEvent( UserLoggedInDomainEvent.Create(Id));
        }

        return Result.Success(isValid);
    }

    public void UpdateLastLogin()
        => LastLoginDate = DateTimeOffset.UtcNow;

    #endregion

    #region Refresh Token Management

    public Result AddRefreshToken(RefreshToken refreshToken)
    {
        if (refreshToken is null)
            return UserErrors.InvalidRefreshToken;

        _refreshTokens.Add(refreshToken);

        return Result.Success();
    }

    public void RevokeAllRefreshTokens()
    {
        foreach (var token in _refreshTokens.Where(t => t.IsActive))
            token.Revoke();
    }

    #endregion

    #region Role Management

    public Result AssignRole(UserRole userRole)
    {
        if (userRole is null)
            return UserErrors.RoleIsNull;

        if (!_userRoles.Add(userRole))
            return UserErrors.UserAlreadyHasRole;

        UpdateSecurityStamp();

        RaiseDomainEvent( RoleAssignedToUserDomainEvent.Create(Id, userRole.Id));

        return Result.Success();
    }

    public Result RemoveRole(UserRole userRole)
    {
        if (userRole is null)
            return UserErrors.RoleIsNull;

        if (!_userRoles.Remove(userRole))
            return UserErrors.RoleNotFound;

        UpdateSecurityStamp();

        RaiseDomainEvent( RoleRemovedFromUserDomainEvent.Create(Id, userRole.Id));

        return Result.Success();
    }

    public Result ClearRoles()
    {
        if (_userRoles.Count == 0)
            return Result.Success();

        _userRoles.Clear();
        UpdateSecurityStamp();

        RaiseDomainEvent( UserRolesClearedDomainEvent.Create(Id));

        return Result.Success();
    }

    public bool HasRole(UserRole userRole) => _userRoles.Contains(userRole);

    #endregion

    #region Private Helpers

    private void UpdateSecurityStamp()
        => SecurityStamp = SecurityStamp.Generate();

    private FullName? BuildFullName()
    {
        if (FirstName is null || LastName is null)
            return null;

        Result<FullName> result = FullName.Create(FirstName.Value, LastName.Value);
        return result.IsSuccess ? result.Value : null;
    }

    #endregion

    public Result UpdateEmailByAdmin(Email emailResultValue)
    {

        if (Email.Equals(emailResultValue))
            return Result.Success();

        Email = emailResultValue;
        UpdateSecurityStamp();

        return Result.Success();
    }
    public Result UpdatePasswordByAdmin(Password passwordResultValue)
    {

        Password =passwordResultValue;
        UpdateSecurityStamp();

        return Result.Success();
    }
    public Result ActivateByAdmin()
    {
        IsActive = true;
        ActiveCode = null;
        UpdateSecurityStamp();

        return Result.Success();

    }


    public Result ManageRoles(List<RoleId> requestedRoleIds)
    {
        // Validation
        if (requestedRoleIds == null)
            return Result.Failure(Error.NullValue);

        // Remove duplicates
        var uniqueRequestedRoles = requestedRoleIds.Distinct().ToHashSet();
        var currentRoleIds = _userRoles.Select(ur => ur.RoleId).ToHashSet();

        // Remove roles
        IEnumerable<RoleId> rolesToRemove = currentRoleIds.Except(uniqueRequestedRoles);
        _userRoles.RemoveWhere(ur => rolesToRemove.Contains(ur.RoleId));

        // Add new roles
        IEnumerable<RoleId> rolesToAdd = uniqueRequestedRoles.Except(currentRoleIds);
        foreach (RoleId roleId in rolesToAdd)
        {
            _userRoles.Add(UserRole.Create(roleId, Id));
        }

        return Result.Success();
    }

}


