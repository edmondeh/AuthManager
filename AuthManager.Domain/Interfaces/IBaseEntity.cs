using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthManager.Domain.Interfaces.Entities
{
    public interface IBaseEntity
    {
        int Id { get; set; }
    }
}
