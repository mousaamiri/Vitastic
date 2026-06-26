using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Api.Extensions;
using Vitastic.Api.Features.Base;
using Vitastic.Api.Features.Users.Requests;
using Vitastic.Api.Features.Users.Responses;
using Vitastic.Api.Wrapper;
using Vitastic.App.Features.Users.Commands.ActivateUser;
using Vitastic.App.Features.Users.Commands.AssignRoleToUser;
using Vitastic.App.Features.Users.Commands.ChangeEmail;
using Vitastic.App.Features.Users.Commands.ChangePassword;
using Vitastic.App.Features.Users.Commands.CreateByAdmin;
using Vitastic.App.Features.Users.Commands.DeactivateUser;
using Vitastic.App.Features.Users.Commands.Login;
using Vitastic.App.Features.Users.Commands.RefreshToken;
using Vitastic.App.Features.Users.Commands.Register;
using Vitastic.App.Features.Users.Commands.RemoveRoleFromUser;
using Vitastic.App.Features.Users.Commands.RequestPasswordReset;
using Vitastic.App.Features.Users.Commands.ResendActivationCode;
using Vitastic.App.Features.Users.Commands.ResetPassword;
using Vitastic.App.Features.Users.Commands.SetUserAvatar;
using Vitastic.App.Features.Users.Commands.UpdateProfile;
using Vitastic.App.Features.Users.Commands.UpdateUserByAdmin;
using Vitastic.App.Features.Users.Queries.CheckUserRole;
using Vitastic.App.Features.Users.Queries.GetAvatarImage;
using Vitastic.App.Features.Users.Queries.GetAvatarInfo;
using Vitastic.App.Features.Users.Queries.GetByEmail;
using Vitastic.App.Features.Users.Queries.GetById;
using Vitastic.App.Features.Users.Queries.GetByUserName;
using Vitastic.App.Features.Users.Queries.Search;
using Vitastic.Domain.Shared.Results;
using ForgotPasswordRequest = Vitastic.Api.Features.Users.Requests.ForgotPasswordRequest;
using ResetPasswordRequest = Vitastic.Api.Features.Users.Requests.ResetPasswordRequest;

namespace Vitastic.Api.Features.Users;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class UsersController(
    IMediator mediator,
    IMapper mapper,
    ILogger<UsersController> logger)
    : ControllerBase
{
    // ======================== COMMANDS ========================

    #region ==================== CREATE USER BY ADMIN ====================

    [Authorize(Policy = "AdminOnly")]
    [HttpPost("by-admin")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<Guid>> CreateUserByAdmin(
        [FromForm] UpsertUserByAdminRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Creating user by admin — UserName: {UserName}, Email: {Email}",
            request.UserName, request.Email);
        if (request.Password is null || request.Password.Length < 6)
            return ApiResponse<Guid>.Fail("رمز عبور کاربر الزامی است. ");
        var command = new CreateUserByAdminCommand(
            request.UserName,
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.AvatarFile!=null? new FormFileAdapter(request.AvatarFile):null,
            request.RoleIds,
            request.IsActive);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Create user by admin failed — {ErrorCode}: {ErrorMessage}",
                result.Error.Code, result.Error.Message);
            return result.ToApiResponse<Guid>("ایجاد کاربر توسط ادمین انجام نشد.");
        }

        logger.LogInformation("User created by admin successfully — UserId: {UserId}", result.Value);

        return result.ToApiResponse(t=>t,"کاربر توسط ادمین با موفقیت ایجاد شد.");
    }

    #endregion

    #region ==================== UPDATE USER BY ADMIN ====================

    [Authorize(Policy = "AdminOnly")]
    [HttpPut("{userId:guid}/by-admin")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> UpdateUserByAdmin(
        [FromRoute] Guid userId,
        [FromForm] UpsertUserByAdminRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Updating user by admin — TargetUserId: {TargetUserId}, UserName: {UserName}, Email: {Email}",
            userId, request.UserName, request.Email);

        var command = new UpdateUserByAdminCommand(
            userId,
            request.UserName,
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName,
            request.PhoneNumber,
            request.AvatarFile!=null? new FormFileAdapter(request.AvatarFile):null,
            request.RoleIds,
            request.IsActive);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Update user by admin failed — TargetUserId: {TargetUserId}, {ErrorCode}: {ErrorMessage}",
                userId, result.Error.Code, result.Error.Message);
            return result.ToApiResponse("به‌روزرسانی کاربر توسط ادمین انجام نشد.");
        }

        logger.LogInformation("User updated by admin successfully — TargetUserId: {TargetUserId}", userId);
        return result.ToApiResponse("کاربر توسط ادمین با موفقیت به‌روزرسانی شد.");
    }

    #endregion


    #region ==================== REGISTER USER ====================

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<Guid>> RegisterUser(
        [FromBody] RegisterUserRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Registering user — UserName: {UserName}, Email: {Email}",
            request.UserName, request.Email);

        var command = new RegisterUserCommand(
            request.UserName,
            request.Email,
            request.Password,
            request.CallbackUrl);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Register user failed — {ErrorCode}: {ErrorMessage}",
                result.Error.Code, result.Error.Message);

            return result.ToApiResponse<Guid>("ثبت‌نام کاربر انجام نشد");
        }

        logger.LogInformation(
            "User registered successfully — UserId: {UserId}",
            result.Value);

        return result.ToApiResponse(t => t, "ثبت‌نام موفقیت‌آمیز. لینک فعالسازی ارسال شد.");
    }

    #endregion

    #region ==================== LOGIN USER ====================

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResponse<AuthTokenResponse>> LoginUser(
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Login attempt — Identifier: {Identifier}",
            request.Identifier);

        var command = new LoginUserCommand(
            request.Identifier,
            request.Password);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Login failed — Identifier: {Identifier}, Error: {ErrorCode}",
                request.Identifier, result.Error.Code);

            return result.ToApiResponse<AuthTokenResponse>("ورود به سیستم انجام نشد");
        }

        logger.LogInformation(
            "Login successful — UserId: {UserId}",
            result.Value.UserId);

        return result.ToApiResponse(
            mapper.Map<AuthTokenResponse>,
            "ورود موفقیت‌آمیز");
    }

    #endregion

    #region ==================== REFRESH TOKEN ====================

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<AuthTokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ApiResponse<AuthTokenResponse>> RefreshToken(
        [FromBody] RefreshTokenRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Token refresh attempt");

        var command = new RefreshTokenCommand(request.RefreshToken);
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Token refresh failed — Error: {Code}",
                result.Error.Code);

            return result.ToApiResponse<AuthTokenResponse>("تازه‌سازی توکن انجام نشد");
        }

        logger.LogInformation(
            "Token refreshed — UserId: {UserId}",
            result.Value.UserId);

        return result.ToApiResponse(
            mapper.Map<AuthTokenResponse>,
            "توکن با موفقیت تازه‌سازی شد.");
    }

    #endregion

    #region ==================== ACTIVATE USER ====================

    [AllowAnonymous]
    [HttpPost("activation")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse> ActivateUser(
        [FromBody] ActivateUserRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Activating user — Email: {Email}",
            request.Email);

        var command = new ActivateUserCommand(
            request.Email,
            request.ActivationCode);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Activate user failed — {ErrorCode}: {ErrorMessage}",
                result.Error.Code, result.Error.Message);

            return result.ToApiResponse("فعال‌سازی حساب کاربری انجام نشد");
        }

        logger.LogInformation(
            "User activated successfully — Email: {Email}",
            request.Email);

        return result.ToApiResponse("حساب کاربری با موفقیت فعال شد.");
    }

    #endregion

    #region ==================== ASSIGN ROLE TO USER ====================

    [HttpPut("{userId:guid}/roles/{roleId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse> AssignRoleToUser(
        [FromRoute] Guid userId,
        [FromRoute] Guid roleId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Assigning role to user — UserId: {UserId}, RoleId: {RoleId}",
            userId, roleId);

        var command = new AssignRoleToUserCommand(userId, roleId);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Assign role to user failed — {ErrorCode}: {ErrorMessage}",
                result.Error.Code, result.Error.Message);

            return result.ToApiResponse("اختصاص نقش به کاربر انجام نشد");
        }

        logger.LogInformation(
            "Role assigned to user successfully — UserId: {UserId}, RoleId: {RoleId}",
            userId, roleId);

        return result.ToApiResponse("نقش با موفقیت به کاربر اختصاص یافت.");
    }

    #endregion

    #region ==================== CHANGE USER EMAIL ====================

    [HttpPatch("{userId:guid}/email")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse> ChangeUserEmail(
        [FromRoute] Guid userId,
        [FromBody] ChangeEmailRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Changing email for user — UserId: {UserId}",
            userId);

        var command = new ChangeEmailCommand(userId, request.NewEmail);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Change email failed — UserId: {UserId}, {ErrorCode}: {ErrorMessage}",
                userId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse("تغییر ایمیل انجام نشد");
        }

        logger.LogInformation(
            "Email changed successfully — UserId: {UserId}",
            userId);

        return result.ToApiResponse("ایمیل با موفقیت تغییر یافت.");
    }

    #endregion

    #region ==================== FORGOT PASSWORD ====================

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse> ForgotPassword(
        [FromBody] ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Password reset requested — Email: {Email}",
            request.Email);

        var command = new RequestPasswordResetCommand(request.Email, request.CallbackUrl);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Password reset request failed — {ErrorCode}: {ErrorMessage}",
                result.Error.Code, result.Error.Message);

            return result.ToApiResponse("درخواست بازیابی رمز عبور انجام نشد");
        }

        logger.LogInformation(
            "Password reset link sent — Email: {Email}",
            request.Email);

        return result.ToApiResponse("لینک بازیابی رمز عبور ارسال شد.");
    }

    #endregion

    #region ==================== RESEND ACTIVATION CODE ====================

    [AllowAnonymous]
    [HttpPost("resend-activation-code")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse> ResendActivationCode(
        [FromBody] ResendActivationRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Password reset requested — Email: {Email}",
            request.Email);

        var command = new ResendActivationCodeCommand(request.Email, request.CallbackUrl);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Password reset request failed — {ErrorCode}: {ErrorMessage}",
                result.Error.Code, result.Error.Message);

            return result.ToApiResponse("درخواست بازیابی رمز عبور انجام نشد");
        }

        logger.LogInformation(
            "Password reset link sent — Email: {Email}",
            request.Email);

        return result.ToApiResponse("لینک بازیابی رمز عبور ارسال شد.");
    }

    #endregion

    #region ==================== RESET PASSWORD ====================

    [AllowAnonymous]
    [HttpPost("reset-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse> ResetPassword(
        [FromBody] ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Password reset attempt with token");

        var command = new ResetPasswordCommand(
            request.Token,
            request.NewPassword);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Password reset failed — {ErrorCode}: {ErrorMessage}",
                result.Error.Code, result.Error.Message);

            return result.ToApiResponse("بازیابی رمز عبور انجام نشد");
        }

        logger.LogInformation("Password reset successfully");

        return result.ToApiResponse("رمز عبور با موفقیت تغییر یافت.");
    }

    #endregion

    #region ==================== CHANGE PASSWORD (FOR LOGEDIN IN USERS) ====================

    [HttpPost("change-password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Password change attempt initiated by user.");

        var userId = User.GetCurrentUserId();
        if (userId is null)
        {
            return ApiResponse.Fail(
                "کاربر لاگین نشده است.",
                ErrorType.Forbidden,
                ["برای تغییر رمز عبور، ابتدا باید وارد حساب کاربری خود شوید."]);
        }

        var command = new ChangePasswordCommand(
            userId.Value,
            request.CurrentPassword,
            request.NewPassword);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Password change failed — {ErrorCode}: {ErrorMessage}",
                result.Error.Code,
                result.Error.Message);

            return result.ToApiResponse("تغییر رمز عبور با شکست مواجه شد.");
        }

        logger.LogInformation("Password changed successfully for user ID: {UserId}", userId.Value);

        return result.ToApiResponse("رمز عبور با موفقیت تغییر یافت.");
    }

    #endregion

    #region ==================== DEACTIVATE USER ====================

    [HttpPatch("{userId:guid}/deactivation")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse> DeactivateUser(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Deactivating user — UserId: {UserId}",
            userId);

        var command = new DeactivateUserCommand(userId);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Deactivate user failed — UserId: {UserId}, {ErrorCode}: {ErrorMessage}",
                userId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse("غیرفعال‌سازی کاربر انجام نشد");
        }

        logger.LogInformation(
            "User deactivated successfully — UserId: {UserId}",
            userId);

        return result.ToApiResponse("کاربر با موفقیت غیرفعال شد.");
    }

    #endregion

    #region ==================== REMOVE ROLE FROM USER ====================

    [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse> RemoveRoleFromUser(
        [FromRoute] Guid userId,
        [FromRoute] Guid roleId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Removing role from user — UserId: {UserId}, RoleId: {RoleId}",
            userId, roleId);

        var command = new RemoveRoleFromUserCommand(userId, roleId);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Remove role from user failed — UserId: {UserId}, RoleId: {RoleId}, {ErrorCode}: {ErrorMessage}",
                userId, roleId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse("حذف نقش از کاربر انجام نشد");
        }

        logger.LogInformation(
            "Role removed from user successfully — UserId: {UserId}, RoleId: {RoleId}",
            userId, roleId);

        return result.ToApiResponse("نقش با موفقیت از کاربر حذف شد.");
    }

    #endregion

    #region ==================== UPDATE USER PROFILE ====================

    [HttpPatch("{userId:guid}/profile")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse> UpdateUserProfile(
        [FromRoute] Guid userId,
        [FromBody] UpdateProfileRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Updating profile for user — UserId: {UserId}",
            userId);

        var command = new UpdateProfileCommand(
            userId,
            request.FirstName,
            request.LastName);

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Update profile failed — UserId: {UserId}, {ErrorCode}: {ErrorMessage}",
                userId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse("به‌روزرسانی پروفایل انجام نشد");
        }

        logger.LogInformation(
            "Profile updated successfully — UserId: {UserId}",
            userId);

        return result.ToApiResponse("پروفایل با موفقیت به‌روزرسانی شد.");
    }

    #endregion

    #region ==================== UPDATE USER AVATAR ====================

    [HttpPatch("{userId:guid}/avatar")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> UpdateUserAvatar(
        [FromRoute] Guid userId,
        IFormFile avatarFile,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Updating avatar for user — UserId: {UserId}",
            userId);

        var command = new SetUserAvatarCommand(userId, new FormFileAdapter(avatarFile));

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Update avatar failed — UserId: {UserId}, {ErrorCode}: {ErrorMessage}",
                userId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse("به‌روزرسانی آواتار انجام نشد");
        }

        logger.LogInformation(
            "Avatar updated successfully — UserId: {UserId}",
            userId);

        return result.ToApiResponse("آواتار با موفقیت به‌روزرسانی شد.");
    }

    #endregion


    // ──────────────────────────────────────────────
    //  QUERIES
    // ──────────────────────────────────────────────

    #region ==================== GET USER BY ID ====================

    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<UserDetailResponse>> GetUserById(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Getting user by ID — UserId: {UserId}",
            userId);

        var query = new GetUserQuery(userId);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Get user by ID failed — UserId: {UserId}, {ErrorCode}: {ErrorMessage}",
                userId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse<UserDetailResponse>("کاربر یافت نشد");
        }

        logger.LogInformation(
            "User retrieved successfully — UserId: {UserId}",
            userId);

        return result.ToApiResponse(
            mapper.Map<UserDetailResponse>,
            "جزئیات کاربر دریافت شد."
        );
    }

    #endregion

    #region ==================== SEARCH USERS ====================

    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<UserResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<PaginatedResponse<UserResponse>>> SearchUsers(
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber=1,
        [FromQuery] int pageSize=10,
        CancellationToken cancellationToken=default)
    {
        logger.LogInformation(
            "Searching users with term: {SearchTerm}, Page: {PageNumber}, PageSize: {PageSize}",
            searchTerm, pageNumber, pageSize);
        var searchUsersQuery = new SearchUsersQuery(searchTerm, pageNumber, pageSize);
        var result = await mediator.Send(searchUsersQuery, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "User search failed — SearchTerm: {SearchTerm}, Error: {ErrorCode} - {ErrorMessage}",
                searchTerm, result.Error.Code, result.Error.Message);

            return result.ToApiResponse<PaginatedResponse<UserResponse>>("نتیجه‌ای یافت نشد.");
        }

        logger.LogInformation(
            "User search completed successfully — Found {ItemCount} items for term: {SearchTerm}",
            result.Value?.Items?.Count ?? 0, searchTerm);

        return result.ToApiResponse(
            mapper.Map<PaginatedResponse<UserResponse>>,
            "لیست کاربران با موفقیت دریافت شد."
        );
    }

    #endregion


    #region ==================== GET USER AVATAR INFO BY ID ====================

    [HttpGet("{userId:guid}/avatar/info")]
    [ProducesResponseType(typeof(ApiResponse<UserAvatarInfoResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<UserAvatarInfoResponse>> GetUserAvatarInfoById(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Getting user avatar info by ID — UserId: {UserId}",
            userId);

        var query = new GetAvatarInfoQuery(userId);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Get user avatar info by ID failed — UserId: {UserId}, {ErrorCode}: {ErrorMessage}",
                userId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse<UserAvatarInfoResponse>("کاربر یافت نشد");
        }

        logger.LogInformation(
            "User retrieved successfully — UserId: {UserId}",
            userId);

        return result.ToApiResponse(
            mapper.Map<UserAvatarInfoResponse>,
            "اطلاعات اواتار این کاربر یافت نشد."
        );
    }

    #endregion

    #region ==================== GET USER AVATAR Image BY ID ====================

    [HttpGet("{userId:guid}/avatar/image")]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<string>> GetUserAvatarImageById(
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Getting user avatar by ID — UserId: {UserId}",
            userId);

        var query = new GetAvatarImageQuery(userId);
        Result<string> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Get user avatar by ID failed — UserId: {UserId}, {ErrorCode}: {ErrorMessage}",
                userId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse<string>("کاربر یافت نشد");
        }

        logger.LogInformation(
            "User retrieved successfully — UserId: {UserId}",
            userId);

        return result.ToApiResponse(
            t => t,
            " اواتار این کاربر یافت نشد."
        );
    }

    #endregion

    #region ==================== GET USER BY EMAIL OR USERNAME ====================

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<UserResponse>> GetUsers(
        [FromQuery] string? email,
        [FromQuery] string? username,
        CancellationToken cancellationToken)
    {
        var hasEmail = !string.IsNullOrWhiteSpace(email);
        var hasUsername = !string.IsNullOrWhiteSpace(username);

        // Validation
        if (hasEmail == hasUsername)
        {
            logger.LogWarning(
                "GetUsers called with invalid parameters — Email: {Email}, Username: {Username}",
                email, username);

            return ApiResponse<UserResponse>.Fail(
                "باید دقیقاً یکی از 'email' یا 'username' ارسال شود.",
                ErrorType.Validation,
                ["InvalidQueryParameters"]
            );
        }

        // Search by email
        if (hasEmail)
        {
            logger.LogInformation("Getting user by email — Email: {Email}", email);

            var query = new GetUserByEmailQuery(email!);
            var result = await mediator.Send(query, cancellationToken);

            if (result.IsFailure)
            {
                logger.LogWarning(
                    "Get user by email failed — {ErrorCode}: {ErrorMessage}",
                    result.Error.Code, result.Error.Message);

                return result.ToApiResponse<UserResponse>("کاربر یافت نشد");
            }

            return result.ToApiResponse(
                mapper.Map<UserResponse>,
                "کاربر یافت شد."
            );
        }

        // Search by username
        logger.LogInformation("Getting user by username — Username: {Username}", username);

        var usernameQuery = new GetUserByUsernameQuery(username!);
        var usernameResult = await mediator.Send(usernameQuery, cancellationToken);

        if (usernameResult.IsFailure)
        {
            logger.LogWarning(
                "Get user by username failed — {ErrorCode}: {ErrorMessage}",
                usernameResult.Error.Code, usernameResult.Error.Message);

            return usernameResult.ToApiResponse(mapper.Map<UserResponse>, "کاربر یافت نشد");
        }

        return usernameResult.ToApiResponse(
            mapper.Map<UserResponse>,
            "کاربر یافت شد."
        );
    }

    #endregion

    #region ==================== CHECK USER ROLE ====================

    [HttpGet("{userId:guid}/roles/{roleId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<bool>> CheckUserRole(
        [FromRoute] Guid userId,
        [FromRoute] Guid roleId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Checking user role — UserId: {UserId}, RoleId: {RoleId}",
            userId, roleId);

        var query = new CheckUserRoleQuery(userId, roleId);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Check user role failed — UserId: {UserId}, RoleId: {RoleId}, {ErrorCode}: {ErrorMessage}",
                userId, roleId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse<bool>("بررسی نقش کاربر انجام نشد");
        }

        var message = result.Value
            ? "کاربر این نقش را دارد."
            : "کاربر این نقش را ندارد.";

        logger.LogInformation(
            "Check user role completed — UserId: {UserId}, RoleId: {RoleId}, HasRole: {HasRole}",
            userId, roleId, result.Value);

        return result.ToApiResponse(t => t, message);
    }

    #endregion
}
