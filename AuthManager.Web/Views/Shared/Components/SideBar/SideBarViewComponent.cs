using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthManager.Web.Views.Shared.Components.SideBar
{
    public class SideBarViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            //var user = _userManager.Users.Select(u => new UserViewModel{
            //    FirstName = u.FirstName,
            //    LastName = u.LastName,
            //    UserName = u.UserName,
            //    ProfilePicture = u.ProfilePicture
            //}).FirstOrDefault(u => u.UserName == HttpContext.User.Identity.Name);
            return View();
        }
    }
}
