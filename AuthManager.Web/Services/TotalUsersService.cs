using AuthManager.Application.Interfaces.Shared;
using AuthManager.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthManager.Web.Services
{
    public class TotalUsersService : ITotalUsersService
    {
        protected DateTime Last7Days = DateTime.Now.AddDays(-7);
        protected DateTime Last15Days = DateTime.Now.AddDays(-15);
        protected DateTime Last1Month = DateTime.Now.AddMonths(-1);

        public TotalUsersService(UserManager<ApplicationUser> userManager)
        {
            OnlineUsers = 1;
            NewUsers = userManager.Users.Where(u => u.CreatedOn >= Last15Days).Count();
            ActiveUsers = userManager.Users.Where(u => u.IsActive == true).Count();
            TotlaUsers = userManager.Users.Count();
        }

        public int OnlineUsers { get; set; }
        public int NewUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotlaUsers { get; set; }
    }
}
