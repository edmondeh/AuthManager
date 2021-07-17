using Microsoft.AspNetCore.Mvc;

namespace AuthManager.Web.Views.Shared.Components.Error
{
    public class ErrorViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}