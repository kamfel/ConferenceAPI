using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceAPI.Core.Models
{
    public class UserRole
    {
        public User User { get; set; }
        public Role Role { get; set; }
    }
}
