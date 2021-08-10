using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthManager.Web.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        //[Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }
        //[Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
        //[Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Bio { get; set; }
        public bool IsActive { get; set; } = true;
        //[Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
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