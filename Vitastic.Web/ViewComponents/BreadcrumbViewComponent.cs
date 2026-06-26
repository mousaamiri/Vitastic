using Microsoft.AspNetCore.Mvc;

namespace Vitastic.Web.ViewComponents;

    public class BreadcrumbViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<BreadcrumbItem> items)
        {
            return View(items);
        }
    }
