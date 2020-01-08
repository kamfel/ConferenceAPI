using System;
using System.Collections.Generic;

namespace ConferenceAPI.Core.Models
{
    public partial class User
    {
        public User()
        {
            Reservations = new HashSet<Reservation>();
            UserPermissions = new HashSet<UserPermission>();
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<UserPermission> UserPermissions { get; set; }
    }
}
