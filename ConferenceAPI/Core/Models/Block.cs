using System;
using System.Collections.Generic;

namespace ConferenceAPI.Core.Models
{
    public partial class Block
    {
        public Block()
        {
            Reservations = new HashSet<Reservation>();
        }

        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public virtual Room RoomNumberNavigation { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
