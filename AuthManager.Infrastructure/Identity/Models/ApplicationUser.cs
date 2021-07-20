
using AuthManager.Domain.Interfaces.Entities;
using Microsoft.AspNetCore.Identity;
using System;

namespace AuthManager.Infrastructure.Identity.Models
{
    public class ApplicationUser : IdentityUser, IAuditableBaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] ProfilePicture { get; set; }
        public bool IsActive { get; set; } = false;
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}