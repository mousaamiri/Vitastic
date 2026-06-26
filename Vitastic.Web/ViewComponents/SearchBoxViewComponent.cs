using Microsoft.AspNetCore.Mvc;

namespace Vitastic.Web.ViewComponents
{
    public class SearchBoxViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string? placeholder = "جستجوی دوره...")
        {
            return View(new { Placeholder = placeholder });
        }
    }
}