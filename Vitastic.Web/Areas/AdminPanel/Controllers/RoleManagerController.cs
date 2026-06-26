using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Areas.AdminPanel.Controllers.Base;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.JsDataTableHelper;
using Vitastic.Web.Models.DTOs.Role;

namespace Vitastic.Web.Areas.AdminPanel.Controllers;

public class RoleManagerController(IRoleManagerService roleService) : AdminController
{
    #region Index

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        PaginatedApiResponse<RoleDto> response = await roleService.GetRolesAsync(null);
        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] = response.Message ?? "خطایی در دریافت نقش ها به وجود آمده است.";
            return View(new PaginatedData<RoleDto>());
        }
        return View(response.Data);
    }
    [HttpPost]
    public async Task<IActionResult> Index([FromBody] DataTablesRequest request)
    {
        var searchTerm = request.Search?.Value ?? "";
        var pageNumber = (request.Start / request.Length) + 1;
        var pageSize = request.Length;

        PaginatedApiResponse<RoleDto> response = await roleService.GetRolesAsync(searchTerm, pageNumber, pageSize);
        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] = response.Message ?? "خطایی در دریافت نقش ها به وجود آمده است.";
            return View(new PaginatedData<RoleDto>());
        }
        return View(response.Data);
    }
    #endregion

    #region Get role details

    [HttpGet("{roleId:guid}")]
    public async Task<IActionResult> GetRoleDetails(Guid roleId)
    {

        ApiResponse<RoleDetailDto> response = await roleService.GetRoleAsync(roleId);
        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] = response.Message ?? "خطایی در دریافت جزئیات نقش به وجود آمده است.";
            return Json(new RoleDetailDto(Guid.Empty, "", []));
        }
        return Json(response.Data);
    }

    #endregion

    #region Permissions

    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions()
    {
        ApiResponse<List<RolePermissionDto>> response = await roleService.GetAllPermissionAsync();
        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] = response.Message ?? "خطایی در دریافت لیست دسترسی‌ها رخ داد.";
            return Json(new RoleDetailDto(Guid.Empty, string.Empty, []));
        }
        return Json(response.Data);
    }

    [HttpGet("{roleId:guid}/permissions")]
    public async Task<IActionResult> GetRolePermissions(Guid roleId)
    {
        ApiResponse<List<RolePermissionDto>> response = await roleService.GetRolePermissionAsync(roleId);
        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] = response.Message ?? "خطایی در دریافت دسترسی‌های نقش رخ داد.";
            return Json(new RoleDetailDto(Guid.Empty, string.Empty, []));
        }
        return Json(response.Data);
    }

    #endregion

    #region Update

    [HttpPost("{roleId:guid}/UpsertRole")]
    public async Task<IActionResult> UpsertRole(Guid roleId, [FromBody] UpsertRoleRequest request)
    {
        var isInsert = roleId == Guid.Empty;
        ApiResponse response;
        if (isInsert)
            response = await roleService.CreateRoleAsync(request);
        else
            response = await roleService.UpdateRoleAsync(roleId, request);

        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] = response.Message ?? "خطایی در بروزرسانی/ایجاد نقش رخ داد.";
            return BadRequest(response.Message);
        }
        return Ok();
    }


    #endregion
}
