using AuthManager.Application.Interfaces.Shared;
using AuthManager.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;

namespace AuthManager.Web.Services
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier) == null ? null : httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
            Username = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name) == null ? null : httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name).Value;
            var user = userManager.Users.FirstOrDefault(u => u.Id == UserId);
            FirstName = user.FirstName;
            LastName = user.LastName;
            CreatedOn = user.CreatedOn;
        }

        public string UserId { get; }
        public string Username { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
        public DateTime CreatedOn { get; }
    }
}