using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Vitastic.Web.Areas.AdminPanel.Controllers.Base;
using Vitastic.Web.Infrastructure.ApiClient;
using Vitastic.Web.Infrastructure.Services;
using Vitastic.Web.Models.DTOs.Wallet;

namespace Vitastic.Web.Areas.AdminPanel.Controllers;

public class UserWalletsManagerController(IWalletService walletService):AdminController
{
    #region Index

    public async Task<IActionResult> IndexAsync(int pageNumber=1,int pageSize=10,CancellationToken ct=default)
    {
        ApiResponse<PaginatedData<WalletDto>> response = await walletService.GetUsersWallets(pageNumber,pageSize, ct);
        if(!response.IsSuccess)
            TempData["ErrorMessage"] = response.Errors.Count>0? response.Errors.First() : response.Message;
        return View(response.Data);
    }

    #endregion

    #region Details
    [HttpGet("Details/{userId:guid}")]
    public async Task<PartialViewResult> DetailsAsync(Guid userId, CancellationToken ct =default)
    {
        var response = await walletService.GetUserTransactionsAsync(userId,1, 50, ct);
        if (!response.IsSuccess)
            TempData["ErrorMessage"] = response.Errors.Count > 0 ? response.Errors.First() : response.Message;
        return PartialView("_WalletTransactions", response.Data);
    }

    #endregion
}
