using Vitastic.App.Common.Abstractions.Messaging;
using Vitastic.App.Common.Abstractions.Services.Base;
using Vitastic.App.Data;
using Vitastic.Domain.Entities.Roles;
using Vitastic.Domain.Entities.Roles.ValueObjects;
using Vitastic.Domain.Entities.Users;
using Vitastic.Domain.Entities.Users.ValueObjects;
using Vitastic.Domain.Shared.Repositories;
using Vitastic.Domain.Shared.Results;
using Vitastic.Domain.Shared.ValueObjects;

namespace Vitastic.App.Features.Users.Commands.UpdateUserByAdmin
{
    public sealed class UpdateUserByAdminCommandHandler(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IFileStorageService fileStorageService,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
        : ICommandHandler<UpdateUserByAdminCommand>
    {
        public async Task<Result> Handle(
            UpdateUserByAdminCommand request,
            CancellationToken cancellationToken)
        {
            #region Validate and create value objects

            // Validate UserId
            var userIdResult = UserId.CreateFrom(request.UserId);
            if (userIdResult.IsFailure) return userIdResult.Error;
            var userId = userIdResult.Value;

            // Validate UserName
            var userNameResult = UserName.Create(request.UserName);
            if (userNameResult.IsFailure) return userNameResult.Error;
            var userName = userNameResult.Value;

            // Validate Email
            var emailResult = Email.Create(request.Email);
            if (emailResult.IsFailure) return emailResult.Error;
            var email = emailResult.Value;

            // Validate FirstName
            var firstNameResult = FirstName.Create(request.FirstName);
            if (firstNameResult.IsFailure) return firstNameResult.Error;
            var firstName = firstNameResult.Value;

            // Validate LastName
            var lastNameResult = LastName.Create(request.LastName);
            if (lastNameResult.IsFailure) return lastNameResult.Error;
            var lastName = lastNameResult.Value;

            // Validate PhoneNumber (optional)
            PhoneNumber? phoneNumber = null;
            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                var phoneResult = PhoneNumber.Create(request.PhoneNumber);
                if (phoneResult.IsFailure) return phoneResult.Error;
                phoneNumber = phoneResult.Value;
            }

            // Validate AvatarFileName (optional)
            UserAvatar? avatar = null;
            if (request.AvatarFile is not null || request.AvatarFile?.Length>0)
            {
                var fileName = await fileStorageService.UploadFileAsync
                    (request.AvatarFile,nameof(User),userId.Value,FileType.Image);
                if (string.IsNullOrEmpty(fileName.Trim()))
                    return Error.Validation("UpdateUserByAdminCommand.UserAvatarNotSaved", "در ذخیره اوتار مشکلی پیش آمده است.");
                var avatarResult = UserAvatar.Create(fileName);
                if (avatarResult.IsFailure) return avatarResult.Error;
                avatar=avatarResult.Value;
            }

            // Validate Password (optional)
            Password? newPassword = null;
            if (!string.IsNullOrEmpty(request.Password))
            {
                var pwdResult = Password.Create(request.Password);
                if (pwdResult.IsFailure) return pwdResult.Error;
                newPassword = pwdResult.Value;
            }

            #endregion

            #region Fetch existing user

            var user = await userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null)
                return UserErrors.UserNotFound; // "کاربر یافت نشد."

            #endregion

            #region Update email (if changed)

            if (!user.Email.Equals(email))
            {
                var emailExists = await userRepository.ExistsByEmailAsync(email, cancellationToken);
                if (emailExists)
                    return UserErrors.EmailAlreadyExists; // "ایمیل وارد شده قبلاً استفاده شده است."

                var updateEmailResult = user.UpdateEmailByAdmin(email);
                if (updateEmailResult.IsFailure)
                    return updateEmailResult.Error;
            }

            #endregion

            #region Update password (if provided)

            if (newPassword is not null)
            {
                var updatePasswordResult = user.UpdatePasswordByAdmin(newPassword);
                if (updatePasswordResult.IsFailure)
                    return updatePasswordResult.Error;
            }

            #endregion

            #region Update profile info

            var profileResult = user.UpdateProfile(firstName, lastName, phoneNumber);
            if (profileResult.IsFailure)
                return profileResult.Error;

            #endregion

            #region Update User name

            if (!user.UserName.Equals(userName))
            {
                var userNameIsExist = await userRepository.ExistsByUserNameAsync(userName, cancellationToken);
                if(userNameIsExist)
                    return  UserErrors.UserNameAlreadyExists;
                var updateUserNameResult = user.UpdateUserName(userName);
                if (updateUserNameResult.IsFailure)
                    return updateUserNameResult.Error;
            }

            #endregion

            #region Update avatar (if provided)

            if (avatar is not null)
            {
                var avatarResult = user.ChangeAvatar(avatar);
                if (avatarResult.IsFailure)
                    return avatarResult.Error;
            }

            #endregion

            #region Manage active/inactive status

            if (request.IsActive && !user.IsActive)
            {
                user.ActivateByAdmin(); // No activation code needed for admin actions
            }
            else if (!request.IsActive && user.IsActive)
            {
                var deactivateResult = user.Deactivate();
                if (deactivateResult.IsFailure)
                    return deactivateResult.Error;
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

            #region Save changes

            await userRepository.UpdateAsync(user, cancellationToken);
            return Result.Success();

            #endregion
        }
    }
}
