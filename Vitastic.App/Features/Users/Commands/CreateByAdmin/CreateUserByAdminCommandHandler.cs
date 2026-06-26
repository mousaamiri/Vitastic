using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.CreateByAdmin;

public sealed class CreateUserByAdminCommandHandler(
    IFileStorageService fileStorageService,
    IUserRepository userRepository,
    IRoleRepository roleRepository)
    : ICommandHandler<CreateUserByAdminCommand,Guid>
{
    public async Task<Result<Guid>> Handle(
        CreateUserByAdminCommand request,
        CancellationToken cancellationToken)
    {
        #region Validate and create value objects

        // Validate UserName
        var userNameResult = UserName.Create(request.UserName);
        if (userNameResult.IsFailure) return userNameResult.Error;

        // Validate Email
        var emailResult = Email.Create(request.Email);
        if (emailResult.IsFailure) return emailResult.Error;

        // Validate Password (required for new user)
        var passwordResult = Password.Create(request.Password);
        if (passwordResult.IsFailure) return passwordResult.Error;

        // Validate FirstName
        var firstNameResult = FirstName.Create(request.FirstName);
        if (firstNameResult.IsFailure) return firstNameResult.Error;

        // Validate LastName
        var lastNameResult = LastName.Create(request.LastName);
        if (lastNameResult.IsFailure) return lastNameResult.Error;

        // Validate PhoneNumber (optional)
        PhoneNumber? phoneNumber = null;
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var phoneResult = PhoneNumber.Create(request.PhoneNumber);
            if (phoneResult.IsFailure) return phoneResult.Error;
            phoneNumber = phoneResult.Value;
        }

        // Validate RoleIds
        var roleIds = new List<RoleId>();
        foreach (var roleIdGuid in request.RoleIds)
        {
            var roleIdResult = RoleId.CreateFrom(roleIdGuid);
            if (roleIdResult.IsFailure)
                return RoleErrors.InvalidRoleId; // "شناسه نقش نامعتبر است."
            var roleIsExist = await roleRepository.RoleIsExistedAsync(roleIdResult.Value, cancellationToken);
            if(!roleIsExist)
                return RoleErrors.RoleNotFound;
            roleIds.Add(roleIdResult.Value);
        }

        #endregion

        #region Check for duplicate email or username

        if (await userRepository.ExistsByEmailAsync(emailResult.Value, cancellationToken))
            return UserErrors.EmailAlreadyExists; // "ایمیل وارد شده قبلاً استفاده شده است."

        if (await userRepository.ExistsByUserNameAsync(userNameResult.Value, cancellationToken))
            return UserErrors.UserNameAlreadyExists; // "نام کاربری وارد شده قبلاً استفاده شده است."

        #endregion

        #region Create new user

        var newUserResult = User.CreateByAdmin(
            userName: userNameResult.Value,
            email: emailResult.Value,
            password: passwordResult.Value,
            firstName: firstNameResult.Value,
            lastName: lastNameResult.Value,
            phoneNumber: phoneNumber,
            avatar: UserAvatar.Default(),
            isActive: request.IsActive);

        if (newUserResult.IsFailure)
            return newUserResult.Error;

        var user = newUserResult.Value;

        #endregion

        #region Set user avatar if exist
        if (request.AvatarFile is not null || request.AvatarFile!.Length>0)
        {
            var fileName = await fileStorageService.UploadFileAsync
                (request.AvatarFile,nameof(User),user.Id,FileType.Image);
            if (string.IsNullOrEmpty(fileName.Trim()))
                return Error.Validation("CreateUserByAdminCommand.UserAvatarNotSaved", "در ذخیره اوتار مشکلی پیش آمده است.");
            var avatarResult = UserAvatar.Create(fileName);
            if (avatarResult.IsFailure) return avatarResult.Error;
            user.ChangeAvatar(avatarResult.Value);
        }
        #endregion

        #region Manage roles

        // Convert input role IDs to RoleId value objects
        var requestedRoleIds = new List<RoleId>();
        foreach (var roleIdGuid in request.RoleIds)
        {
            var roleIdResult = RoleId.CreateFrom(roleIdGuid);
            if (roleIdResult.IsFailure)
                return RoleErrors.InvalidRoleId; // "شناسه نقش نامعتبر است."
            var roleIsExist = await roleRepository.RoleIsExistedAsync(roleIdResult.Value, cancellationToken);
            if(!roleIsExist)
                return RoleErrors.RoleNotFound;
            requestedRoleIds.Add(roleIdResult.Value);
        }

        Result clearResult = user.ManageRoles(requestedRoleIds);
        if(clearResult.IsFailure)
            return clearResult.Error;

        #endregion

        #region Save user

        // UnitOfWorkBehavior will commit the transaction
        await userRepository.AddAsync(user, cancellationToken);
        return Result.Success(user.Id.Value);

        #endregion
    }
}
