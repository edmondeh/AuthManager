using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthManager.Web.Areas.Admin.Models
{
    public class PermissionViewModel
    {
        public string RoleId { get; set; }
        public IList<RoleClaimsViewModel> RoleClaims { get; set; }
    }

    public class RoleClaimsViewModel
    {
        public string Type { get; set; }
        public string Value { get; set; }
        public bool Selected { get; set; }
    }

    public class PermissionCreateViewModel
    {
        public string Type { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public string Module { get; set; }
    }

    public class PermissionEditViewModel
    {
        public string Type { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public string Module { get; set; }

        public string ClaimValue
        {
            get { return $"Permissions.{Module}.{Value}"; }
            private set { }
        }
    }
}