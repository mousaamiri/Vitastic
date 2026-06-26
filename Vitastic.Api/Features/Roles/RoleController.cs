using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vitastic.Api.Extensions;
using Vitastic.Api.Features.Base;
using Vitastic.Api.Features.Roles.Requests;
using Vitastic.Api.Features.Roles.Responses;
using Vitastic.App.Features.Common.Dtos;
using Vitastic.App.Features.Roles.Commands.AddPermissionToRole;
using Vitastic.App.Features.Roles.Commands.Create;
using Vitastic.App.Features.Roles.Commands.RemovePermissionFromRole;
using Vitastic.App.Features.Roles.Commands.UpdateByAdmin;
using Vitastic.App.Features.Roles.Commands.UpdateName;
using Vitastic.App.Features.Roles.Dtos;
using Vitastic.App.Features.Roles.Queries.CheckRolePermission;
using Vitastic.App.Features.Roles.Queries.CheckRolePermissionByPermissionCode;
using Vitastic.App.Features.Roles.Queries.GetById;
using Vitastic.App.Features.Roles.Queries.GetByName;
using Vitastic.App.Features.Roles.Queries.List;
using Vitastic.App.Features.Roles.Queries.PermissionsList;
using Vitastic.Domain.Shared.Results;

namespace Vitastic.Api.Features.Roles;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class RolesController(
    IMediator mediator,
    IMapper mapper,
    ILogger<RolesController> logger) : ControllerBase
{
    // ======================== COMMANDS ========================

    #region ==================== CREATE ROLE ====================

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse<Guid>> CreateRole(
        [FromBody] CreateRoleRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating Role - Name: {Name}", request.RoleName);

        // Map request to command
        var command = new CreateRoleCommand(request.RoleName, request.PermissionIds);
        var result = await mediator.Send(command, cancellationToken);

        // Handle failure
        if (result.IsFailure)
        {
            logger.LogWarning(
                "Create Role failed - {ErrorCode}: {ErrorMessage}",
                result.Error.Code, result.Error.Message);

            return result.ToApiResponse<Guid>("ایجاد نقش انجام نشد");
        }

        logger.LogInformation("Role created - Id: {RoleId}", result.Value);

        return result
            .ToApiResponse(t => t, "نقش با موفقیت ایجاد شد.");
    }

    #endregion
    #region ==================== UPDATE ROLE ====================

    [HttpPut("{roleId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> UpdateRole(
        Guid roleId,
        [FromBody] UpdateRoleRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating Role - Id: {RoleId}, Name: {Name}", roleId, request.RoleName);

        var command = new UpdateRoleByAdminCommand
        {
            RoleId = roleId,
            RoleName = request.RoleName,
            PermissionIds = request.PermissionIds ?? []
        };

        var result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogWarning(
                "Update Role failed - RoleId: {RoleId}, {ErrorCode}: {ErrorMessage}",
                roleId, result.Error.Code, result.Error.Message);

            return result.ToApiResponse("بروزرسانی نقش انجام نشد");
        }

        logger.LogInformation("Role updated successfully - Id: {RoleId}", roleId);
        return ApiResponse.Success("نقش با موفقیت بروزرسانی شد.");
    }

    #endregion

    #region ==================== UPDATE ROLE NAME ====================

    [HttpPatch("{roleId:guid}/name")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> UpdateRoleName(
        [FromRoute] Guid roleId,
        [FromBody] UpdateRoleNameRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating Role name - Id: {RoleId}", roleId);

        // Map to command and inject roleId
        var command = mapper.Map<UpdateRoleNameCommand>(request) with { RoleId = roleId };
        var result = await mediator.Send(command, cancellationToken);

        // Handle failure
        if (result.IsFailure)
        {
            logger.LogWarning(
                "Update Role name failed - Id: {RoleId}, Error: {ErrorCode}",
                roleId, result.Error.Code);

            return result.ToApiResponse("به‌روزرسانی نام نقش انجام نشد");
        }

        logger.LogInformation("Role name updated - Id: {RoleId}", roleId);

        return result.ToApiResponse("نام نقش با موفقیت به‌روزرسانی شد.");
    }

    #endregion

    #region ==================== ADD PERMISSION TO ROLE ====================

    [HttpPost("{roleId:guid}/permissions")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> AddPermissionToRole(
        Guid roleId,
        [FromBody] AddPermissionToRoleRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Adding permission to role - RoleId: {RoleId}, PermissionId: {PermissionId}",
            roleId, request.PermissionId);

        // Map to command and inject roleId
        var command = mapper.Map<AddPermissionToRoleCommand>(request) with { RoleId = roleId };
        var result = await mediator.Send(command, cancellationToken);

        // Handle failure
        if (result.IsFailure)
        {
            logger.LogWarning(
                "Add permission to role failed - RoleId: {RoleId}, Error: {ErrorCode}",
                roleId, result.Error.Code);

            return result.ToApiResponse("افزودن دسترسی به نقش انجام نشد");
        }

        logger.LogInformation(
            "Permission added to role - RoleId: {RoleId}, PermissionId: {PermissionId}",
            roleId, request.PermissionId);

        return result.ToApiResponse("دسترسی با موفقیت به نقش اضافه شد.");
    }

    #endregion

    #region ==================== REMOVE PERMISSION FROM ROLE ====================

    [HttpDelete("{roleId:guid}/permissions/{permissionId:guid}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> RemovePermissionFromRole(
        Guid roleId,
        Guid permissionId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Removing permission from role - RoleId: {RoleId}, PermissionId: {PermissionId}",
            roleId, permissionId);

        var command = new RemovePermissionFromRoleCommand(roleId, permissionId);
        var result = await mediator.Send(command, cancellationToken);

        // Handle failure
        if (result.IsFailure)
        {
            logger.LogWarning(
                "Remove permission from role failed - RoleId: {RoleId}, Error: {ErrorCode}",
                roleId, result.Error.Code);

            return result.ToApiResponse("حذف دسترسی از نقش انجام نشد");
        }

        logger.LogInformation(
            "Permission removed from role - RoleId: {RoleId}, PermissionId: {PermissionId}",
            roleId, permissionId);

        return result.ToApiResponse("دسترسی با موفقیت از نقش حذف شد.");
    }

    #endregion

    // ======================== QUERIES ========================

    #region ==================== GET ROLE ====================

    [HttpGet("{roleId:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RoleDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<RoleDetailResponse>> GetRole(
        Guid roleId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting Role - Id: {RoleId}", roleId);

        var result = await mediator.Send(
            new GetRoleQuery(roleId), cancellationToken);

        // Handle failure
        if (result.IsFailure)
        {
            logger.LogWarning("Role not found - Id: {RoleId}", roleId);

            return result.ToApiResponse<RoleDetailResponse>("نقش یافت نشد");
        }


        logger.LogInformation("Role retrieved - Id: {RoleId}", roleId);

        return result
            .ToApiResponse(mapper.Map<RoleDetailResponse>, "نقش با موفقیت بازیابی شد.");
    }

    #endregion

    #region ==================== GET ROLE BY NAME ====================

    [HttpGet("by-name/{name:required}")]
    [ProducesResponseType(typeof(ApiResponse<RoleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<RoleResponse>> GetRoleByName(
        string name,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting Role - Name: {RoleName}", name);

        var result = await mediator.Send(
            new GetRoleByNameQuery(name), cancellationToken);

        // Handle failure
        if (result.IsFailure)
        {
            logger.LogWarning("Role not found - Name: {RoleName}", name);

            return result.ToApiResponse<RoleResponse>("نقش یافت نشد");
        }

        logger.LogInformation("Role retrieved - Name: {RoleName}", name);

        return result
            .ToApiResponse(mapper.Map<RoleResponse>, "نقش با موفقیت بازیابی شد.");
    }

    #endregion

    #region ==================== CHECK ROLE HAS PERMISSION ====================

    [HttpGet("{roleId:guid}/permissions/{permissionId:guid}/existence")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<bool>> CheckRoleHasPermission(
        Guid roleId,
        Guid permissionId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Checking role permission - RoleId: {RoleId}, PermissionId: {PermissionId}",
            roleId, permissionId);

        var result = await mediator.Send(
            new CheckRolePermissionQuery(roleId, permissionId), cancellationToken);

        // Handle failure
        if (result.IsFailure)
        {
            logger.LogWarning(
                "Check role permission failed - RoleId: {RoleId}, Error: {ErrorCode}",
                roleId, result.Error.Code);

            return result.ToApiResponse<bool>("بررسی دسترسی انجام نشد");
        }

        logger.LogInformation(
            "Role permission check - RoleId: {RoleId}, PermissionId: {PermissionId}, HasPermission: {HasPermission}",
            roleId, permissionId, result.Value);

        return result
            .ToApiResponse(t => t, "بررسی دسترسی با موفقیت انجام شد.");
    }

    #endregion

    #region ==================== CHECK ROLE HAS PERMISSION BY CODE ====================

    [HttpGet("{roleId:guid}/permissions/by-code/{permissionCode:required}/existence")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<bool>> CheckRoleHasPermissionByCode(
        Guid roleId,
        string permissionCode,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Checking role permission by code - RoleId: {RoleId}, Code: {PermissionCode}",
            roleId, permissionCode);

        var result = await mediator.Send(
            new CheckRolePermissionByPermissionCodeQuery(roleId, permissionCode), cancellationToken);

        // Handle failure
        if (result.IsFailure)
        {
            logger.LogWarning(
                "Check role permission failed - RoleId: {RoleId}, Error: {ErrorCode}",
                roleId, result.Error.Code);

            return result.ToApiResponse<bool>("بررسی دسترسی انجام نشد");
        }

        logger.LogInformation(
            "Role permission check - RoleId: {RoleId}, Code: {PermissionCode}, HasPermission: {HasPermission}",
            roleId, permissionCode, result.Value);

        return result
            .ToApiResponse(t => t, "بررسی دسترسی با موفقیت انجام شد.");
    }

    #endregion

    #region ==================== LIST ROLES ====================

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PaginatedResponse<RoleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ApiResponse<PaginatedResponse<RoleResponse>>> ListRoles(
        [FromQuery] string? searchTerm,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Listing Roles - Page: {Page}, Size: {Size}",
            pageNumber, pageSize);

        Result<PaginatedResult<RoleDto>> result = await mediator.Send(
            new ListRolesQuery(searchTerm,pageNumber, pageSize), cancellationToken);

        // Handle failure
        if (result.IsFailure)
            return result.ToApiResponse<PaginatedResponse<RoleResponse>>("دریافت لیست نقش‌ها انجام نشد");

        // Map result to pagination DTO
        var response = mapper.Map<PaginatedResponse<RoleResponse>>(result.Value);

        logger.LogInformation(
            "Roles listed - Count: {Count}, Page: {Page}/{Total}",
            response.Items.Count, pageNumber, result.Value.TotalPages);

        return result
            .ToApiResponse(mapper.Map<PaginatedResponse<RoleResponse>>, "لیست نقش‌ها با موفقیت بازیابی شد.");
    }

    #endregion

    #region ==================== LIST ROLE PERMISSIONS ====================

    [HttpGet("{roleId:guid}/permissions")]
    [ProducesResponseType(typeof(ApiResponse<List<RolePermissionResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<ApiResponse<List<RolePermissionResponse>>> ListRolePermissions(
        Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new ListRolePermissionsQuery(roleId), cancellationToken);

        if (result.IsFailure)
            return result.ToApiResponse<List<RolePermissionResponse>>("دریافت لیست دسترسی‌های نقش انجام نشد");

        var response = mapper.Map<List<RolePermissionResponse>>(result.Value);

        logger.LogInformation("Role permissions listed - RoleId: {RoleId}, Count: {Count}", roleId, response.Count);

        return ApiResponse<List<RolePermissionResponse>>.Success(response, "لیست دسترسی‌های نقش با موفقیت بازیابی شد.");
    }

    #endregion


}
