using AuthManager.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthManager.Web.Views.Shared.Components.SideBar
{
    public class SideBarViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public SideBarViewComponent(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public IViewComponentResult Invoke()
        {
            var user = _userManager.Users.FirstOrDefault(u => u.UserName == HttpContext.User.Identity.Name);
            return View(user);
        }
    }
}
