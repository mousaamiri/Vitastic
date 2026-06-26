using System.Text.Json.Serialization;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Events;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.Domain.Entities.Users.Events;

public sealed record UserUpdatedDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public UserUpdatedDomainEvent(Guid userId) => UserId = userId;

    public static UserUpdatedDomainEvent Create(UserId userId)
    {

        return new UserUpdatedDomainEvent(userId.Value);

    }
}
public sealed record UserRolesClearedDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public UserRolesClearedDomainEvent(Guid userId) => UserId = userId;

    public static UserRolesClearedDomainEvent Create(UserId userId)
    {

        return new(userId.Value);

    }
}


public sealed record UserRegisteredDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
    public string ActivateCode { get; init; }
    public string CallbackBaseUrl { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public UserRegisteredDomainEvent(Guid userId, string userName, string email,
        string activateCode, string callbackBaseUrl)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        ActivateCode = activateCode;
        CallbackBaseUrl = callbackBaseUrl;
    }

    public static UserRegisteredDomainEvent Create(UserId userId, UserName userName, Email email,
        ActiveCode activateCode, string callbackBaseUrl)
    {

        return new(userId.Value, userName.Value, email.Value, activateCode.Value, callbackBaseUrl);

    }
}

public sealed record UserPasswordResetDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public UserPasswordResetDomainEvent(Guid userId) => UserId = userId;

    public static UserPasswordResetDomainEvent Create(UserId userId)
    {

        return new(userId.Value);

    }
}

public sealed record UserPasswordChangedDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public UserPasswordChangedDomainEvent(Guid userId) => UserId = userId;

    public static UserPasswordChangedDomainEvent Create(UserId userId)
    {

        return new(userId.Value);

    }
}

public sealed record UserLoggedInDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public UserLoggedInDomainEvent(Guid userId) => UserId = userId;

    public static UserLoggedInDomainEvent Create(UserId userId)
    {

        return new(userId.Value);

    }
}

public sealed record UserEmailChangedDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }
    public string OldEmailValue { get; init; }
    public string EmailValue { get; init; }
    public string ActiveCodeCode { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public UserEmailChangedDomainEvent(Guid userId, string oldEmailValue, string emailValue,
        string activeCodeCode)
    {
        UserId = userId;
        OldEmailValue = oldEmailValue;
        EmailValue = emailValue;
        ActiveCodeCode = activeCodeCode;
    }

    public static UserEmailChangedDomainEvent Create(UserId userId, Email oldEmail, Email newEmail,
        ActiveCode activeCode)
    {

        return new(userId.Value, oldEmail.Value, newEmail.Value, activeCode.Value);

    }
}

public sealed record UserDeactivatedDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public UserDeactivatedDomainEvent(Guid userId) => UserId = userId;

    public static UserDeactivatedDomainEvent Create(UserId userId)
    {

        return new(userId.Value);

    }
}

public sealed record UserCreatedByAdminDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public UserCreatedByAdminDomainEvent(Guid userId) => UserId = userId;

    public static UserCreatedByAdminDomainEvent Create(UserId userId)
    {

        return new(userId.Value);

    }
}

public sealed record UserAvatarChangedDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }
    public string OldAvatarFileName { get; init; }
    public string NewAvatarFileName { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public UserAvatarChangedDomainEvent(Guid userId, string oldAvatarFileName, string newAvatarFileName)
    {
        UserId = userId;
        OldAvatarFileName = oldAvatarFileName;
        NewAvatarFileName = newAvatarFileName;
    }

    public static UserAvatarChangedDomainEvent Create(UserId userId, UserAvatar oldAvatar,
        UserAvatar newAvatar)
    {

        return new(userId.Value, oldAvatar.Value, newAvatar.Value);

    }
}

public sealed record UserActivatedDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public UserActivatedDomainEvent(Guid userId) => UserId = userId;

    public static UserActivatedDomainEvent Create(UserId userId)
    {

        return new(userId.Value);

    }
}

public sealed record RoleRemovedFromUserDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }
    public Guid UserRoleId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public RoleRemovedFromUserDomainEvent(Guid userId, Guid userRoleId)
    {
        UserId = userId;
        UserRoleId = userRoleId;
    }

    public static RoleRemovedFromUserDomainEvent Create(UserId userId, UserRoleId userRoleId)
    {

        return new(userId.Value, userRoleId.Value);

    }
}

public sealed record RoleAssignedToUserDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }
    public Guid UserRoleId { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public RoleAssignedToUserDomainEvent(Guid userId, Guid userRoleId)
    {
        UserId = userId;
        UserRoleId = userRoleId;
    }

    public static RoleAssignedToUserDomainEvent Create(UserId userId, UserRoleId userRoleId)
    {

        return new(userId.Value, userRoleId.Value);

    }
}

public sealed record PasswordResetRequestedDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
    public string ResetPasswordToken { get; init; }
    public string CallbackBaseUrl { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public PasswordResetRequestedDomainEvent(Guid userId, string userName, string email,
        string resetPasswordToken, string callbackBaseUrl)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        ResetPasswordToken = resetPasswordToken;
        CallbackBaseUrl = callbackBaseUrl;
    }

    public static PasswordResetRequestedDomainEvent Create(UserId userId, UserName userName, Email email,
        ActiveCode resetPasswordToken, string callbackBaseUrl)
    {
        return new(userId.Value, userName.Value, email.Value, resetPasswordToken.Code, callbackBaseUrl);
    }
}


public sealed record ActivationCodeRegeneratedDomainEvent : DomainEvent
{
    public Guid UserId { get; init; }
    public string UserName { get; init; }
    public string Email { get; init; }
    public string ActiveCode { get; init; }
    public string CallbackUrl { get; init; }

    [JsonConstructor]
    [Obsolete("Use Create() factory method. This constructor is for deserialization only.")]
    public ActivationCodeRegeneratedDomainEvent(Guid userId, string userName, string email,
        string activeCode, string callbackUrl)
    {
        UserId = userId;
        UserName = userName;
        Email = email;
        ActiveCode = activeCode;
        CallbackUrl = callbackUrl;
    }

    public static ActivationCodeRegeneratedDomainEvent Create(UserId userId, UserName userName, Email email,
        ActiveCode activeCode, string callbackUrl)
    {
        return new(userId.Value, userName.Value, email.Value, activeCode.Value, callbackUrl);
    }
}
