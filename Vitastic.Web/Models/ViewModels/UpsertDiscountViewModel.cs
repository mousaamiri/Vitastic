using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Vitastic.Web.Areas.AdminPanel.Controllers;
using Vitastic.Web.Models.DTOs.Disocunt;

namespace Vitastic.Web.Models.ViewModels;

public sealed class UpsertDiscountViewModel
{
    public DiscountDetailDto Discount { get; set; }
    public Guid? DiscountId { get; set; }
}
