using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Api.Extensions;
using Vitastic.Api.Features.Base;
using Vitastic.Api.Features.Permissions.Requests;
using Vitastic.Api.Features.Permissions.Responses;
using Vitastic.Api.Features.Roles.Responses;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Permissions.Commands.Create;
using Vitastic.App.Features.Permissions.Commands.Update;
using Vitastic.App.Features.Permissions.Dtos;
using Vitastic.App.Features.Permissions.Queries.GetByCode;
using Vitastic.App.Features.Permissions.Queries.GetById;
using Vitastic.App.Features.Permissions.Queries.List;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Api.Features.Permissions;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class PermissionsController(
    IMediator mediator,
    IMapper mapper,
    ILogger<PermissionsController> logger) : ControllerBase
{
    // ======================== COMMANDS ========================

    #region ==================== CREATE PERMISSION ====================

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse<Guid>> CreatePermission(
        [FromBody] CreatePermissionRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Creating Permission - Code: {Code}", request.Code);

        // Map incoming request to domain command
        var command = mapper.Map<CreatePermissionCommand>(request);
        var result = await mediator.Send(command, cancellationToken);

        // Handle failure with localized user message
        if (result.IsFailure)
        {
            logger.LogWarning(
                "Create Permission failed - {ErrorCode}: {ErrorMessage}",
                result.Error.Code, result.Error.Message);

            return result.ToApiResponse<Guid>( "ایجاد دسترسی انجام نشد");
        }

        logger.LogInformation("Permission created - Id: {PermissionId}", result.Value);

        return result
            .ToApiResponse(t => t, "دسترسی با موفقیت ایجاد شد.");
    }

    #endregion

    #region ==================== UPDATE PERMISSION ====================

    [HttpPut("{permissionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> UpdatePermission(
        Guid permissionId,
        [FromBody] UpdatePermissionRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Updating Permission - Id: {PermissionId}", permissionId);

        // Build command from parameters and request
        var command = new UpdatePermissionCommand(permissionId, request.Code, request.Description);
        var result = await mediator.Send(command, cancellationToken);

        // Failure handling
        if (result.IsFailure)
        {
            logger.LogWarning(
                "Update Permission failed - Id: {PermissionId}, Error: {ErrorCode}",
                permissionId, result.Error.Code);

            return result.ToApiResponse("به‌روزرسانی دسترسی انجام نشد");
        }

        logger.LogInformation("Permission updated - Id: {PermissionId}", permissionId);

        return result.ToApiResponse("دسترسی با موفقیت به‌روزرسانی شد.");
    }

    #endregion

// ======================== QUERIES ========================

    #region ==================== GET PERMISSION BY ID ====================

    [HttpGet("{permissionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<PermissionResponse>> GetPermissionById(
        Guid permissionId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting Permission - Id: {PermissionId}", permissionId);

        var result = await mediator.Send(
            new GetPermissionByIdQuery(permissionId), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Permission not found - Id: {PermissionId}", permissionId);

            return result.ToApiResponse<PermissionResponse>("دسترسی یافت نشد");
        }

        logger.LogInformation("Permission retrieved - Id: {PermissionId}", permissionId);

        return result
            .ToApiResponse(mapper.Map<PermissionResponse>, "دسترسی با موفقیت بازیابی شد.");
    }

    #endregion

    #region ==================== GET PERMISSION BY CODE ====================

    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(ApiResponse<PermissionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<PermissionResponse>> GetPermissionByCode(
        string code,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting Permission - Code: {Code}", code);

        var result = await mediator.Send(
            new GetPermissionByCodeQuery(code), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Permission not found - Code: {Code}", code);

            return result.ToApiResponse<PermissionResponse>( "دسترسی یافت نشد");
        }

        logger.LogInformation("Permission retrieved - Code: {Code}", code);

        return result
            .ToApiResponse(mapper.Map<PermissionResponse>, "دسترسی با موفقیت بازیابی شد.");
    }

    #endregion

    #region ==================== GET ALL PERMISSIONS ====================

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<PermissionResponse>>), StatusCodes.Status200OK)]
    public async Task<ApiResponse<List<PermissionResponse>>> GetAllPermissions(
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching all permissions for role assignment");

        Result<List<RolePermissionDto>> result =
            await mediator.Send(new GetPermissionsListQuery(), cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning("Failed to fetch permissions");
            return ApiResponse<List<PermissionResponse>>.Fail("خطا در دریافت لیست دسترسی‌ها");
        }

        logger.LogInformation("All permissions retrieved successfully");
        return ApiResponse<List<PermissionResponse>>.Success(
            mapper.Map<List<PermissionResponse>>(result.Value),
            "لیست دسترسی‌ها با موفقیت دریافت شد.");
    }

    #endregion

}
