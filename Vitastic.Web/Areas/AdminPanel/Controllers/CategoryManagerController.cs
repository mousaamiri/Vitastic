using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Areas.AdminPanel.Controllers.Base;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Category;

namespace Vitastic.Web.Areas.AdminPanel.Controllers;

public class CategoryManagerController(ICategoryManagerService categoryService) : AdminController
{
    #region Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        ApiResponse<List<CategoryDetailDto>> result = await categoryService.GetAllAsync();
        if (!result.IsSuccess)
        {
            TempData["ErrorMessage"] = result.Message;
            return View();
        }
        return View(result.Data);
    }

    #endregion
    #region Isert

    [HttpPost("Insert")]
    public async Task<IActionResult> InsertNew(
        [FromBody]UpsertCategoryRequest updates,
        CancellationToken cancellationToken)
    {
        ApiResponse response = await categoryService.CreateNewAsync(updates, cancellationToken);
        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] = response.Message;
            return BadRequest(response.Message);
        }
        TempData["SuccessMessage"] = "دسته‌بندی با موفقیت اضافه شد";
        return Ok();
    }


    #endregion
    #region Update range

    [HttpPost("update-range")]
    public async Task<IActionResult> UpdateRange(
        [FromBody] List<CategoryUpdateDto> updates,
        CancellationToken cancellationToken)
    {
        ApiResponse response = await categoryService.UpdateListAsync(updates, cancellationToken);
        if (!response.IsSuccess)
        {
            TempData["ErrorMessage"] =response.Message;
            return BadRequest(response.Message);
        }
        TempData["SuccessMessage"] = "تغییرات با موفقیت ذخیره شد";
        return Ok(new { success = true });
    }


    #endregion
    #region Delete

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        ApiResponse response = await categoryService.DeleteAsync(id, cancellationToken);
        if (!response.IsSuccess)
        {
            return BadRequest(new { message = response.Message });
        }

        return Ok(new { success = true });
    }

    #endregion

}
