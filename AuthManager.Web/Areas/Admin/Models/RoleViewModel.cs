using System.ComponentModel.DataAnnotations;

namespace AuthManager.Web.Areas.Admin.Models
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }
        public int NumberOfUsers { get; set; }
    }
}