using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthManager.Application.Interfaces.Shared
{
    public interface ITotalUsersService
    {
        public int OnlineUsers { get; set; }
        public int NewUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotlaUsers { get; set; }
    }
}
