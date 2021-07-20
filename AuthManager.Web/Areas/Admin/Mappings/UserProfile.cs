using AuthManager.Infrastructure.Identity.Models;
using AuthManager.Web.Areas.Admin.Models;
using AutoMapper;

namespace AuthManager.Web.Areas.Admin.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserViewModel>().ReverseMap();
        }
    }
}