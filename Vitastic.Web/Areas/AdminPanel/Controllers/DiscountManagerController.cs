using Microsoft.AspNetCore.Mvc;
using Vitastic.Web.Areas.AdminPanel.Controllers.Base;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Disocunt;
using Vitastic.Web.Models.ViewModels;

namespace Vitastic.Web.Areas.AdminPanel.Controllers;

public class DiscountManagerController(IDiscountService discountService) : AdminController
{
    #region Index

    [HttpGet]
    public async Task<IActionResult> IndexAsync(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        ApiResponse<PaginatedData<DiscountDto>> response = await discountService.ListDiscountsAsync(pageNumber, pageSize, ct);
        if (!response.IsSuccess)
            TempData["ErrorMessage"] = response.Errors.Count > 0 ? response.Errors[0] : response.Message;
        return View(response.Data);
    }

    #endregion

    #region Upsert

    [HttpGet("Upsert/{id?}")]
    public async Task<IActionResult> Upsert(Guid? id, CancellationToken ct = default)
    {
        var vm = new UpsertDiscountViewModel { DiscountId = id,Discount = new DiscountDetailDto
        {
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(7)
        }};

        // اگر id داشتیم، باید اطلاعات کد تخفیف رو بگیریم
        if (id.HasValue)
        {
            var response = await discountService.GetDiscountByIdAsync(id.Value, ct);
            if (!response.IsSuccess)
            {
                TempData["ErrorMessage"] = response.Errors.Count > 0 ? response.Errors[0] : response.Message;
                return RedirectToAction("IndexAsync");
            }

            vm.Discount.Code = response.Data!.Code;
            vm.Discount.Title = response.Data.Title;
            vm.Discount.Type = response.Data.Type;
            vm.Discount.Value = response.Data.Value;
            vm.Discount.StartDate = response.Data.StartDate;
            vm.Discount.EndDate = response.Data.EndDate;
        }

        return PartialView("_Upsert", vm);
    }

    [HttpPost("Upsert/{id?}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upsert(Guid? id,UpsertDiscountViewModel model, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(new { success = false, message = "اطلاعات وارد شده معتبر نیست." });
        var requestModel = new UpsertDiscountRequest
        {
            Id = id,
            Code = model.Discount.Code,
            Title = model.Discount.Title,
            
            Amount = (string.Equals(model.Discount.Type, "Percentage", StringComparison.Ordinal)) ? null: model.Discount.Value,
            Percentage = (string.Equals(model.Discount.Type, "Percentage", StringComparison.Ordinal)) ? model.Discount.Value:null,
            StartDate = model.Discount.StartDate,
            EndDate = model.Discount.EndDate

        };
        ApiResponse response;

        if (model.DiscountId.HasValue)
        {
            // Update
            response = await discountService.UpdateDiscountAsync(requestModel, ct);
        }
        else
        {
            // Create
            response =
                model.Discount.Type.Equals("Percentage")
                    ? await discountService.CreatePercentageDiscountAsync(requestModel, ct)
                    : await discountService.CreateFixedDiscountAsync(requestModel, ct);
        }

        if (!response.IsSuccess)
            return BadRequest(new { success = false, message = response.Errors.Count > 0 ? response.Errors.First() : response.Message });

        return Ok(new { success = true, message = model.DiscountId.HasValue ? "کد تخفیف با موفقیت ویرایش شد." : "کد تخفیف با موفقیت ایجاد شد." });
    }

    #endregion
}
