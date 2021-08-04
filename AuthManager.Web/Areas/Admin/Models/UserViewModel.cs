using System;
using System.Collections.Generic;

namespace AuthManager.Web.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Bio { get; set; }
        public bool IsActive { get; set; } = true;
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public byte[] ProfilePicture { get; set; }
        public bool EmailConfirmed { get; set; }
        public IEnumerable<string> RoleNames { get; set; }
        public IEnumerable<string> NewRoleNames { get; set; }
        public DateTime CreatedOn { get; set; }

        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}