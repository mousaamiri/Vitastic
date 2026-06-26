using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Vitastic.Web.Controllers
{
    [AllowAnonymous]
    public class PaymentController : Controller
    {
        #region Result (GET) — Show payment result

        [HttpGet("result")]
        [AllowAnonymous]
        public IActionResult Result([FromQuery] bool success)
        {
            ViewBag.IsSuccess = success;
            return View();
        }

        #endregion
    }
}
