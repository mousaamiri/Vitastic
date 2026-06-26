using Microsoft.AspNetCore.Mvc;

namespace Vitastic.Web.Controllers;

public class ErrorController : Controller
{
    [HttpGet("Error")]
    public IActionResult Index(int code)
    {
        ViewBag.ErrorCode = code;
        ViewBag.ErrorMessage = code switch
        {
            404 => "صفحه مورد نظر یافت نشد",
            403 => "شما دسترسی به این صفحه ندارید",
            401 => "لطفا ابتدا وارد شوید",
            _ => "خطایی رخ داده است"
        };

        return View();
    }

}
