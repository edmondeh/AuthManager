using AuthManager.Domain.Interfaces.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthManager.Domain.Entities
{
    public abstract class AuditableEntity : IAuditableBaseEntity
    {
        protected AuditableEntity()
        {
        }

        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

    }
}
