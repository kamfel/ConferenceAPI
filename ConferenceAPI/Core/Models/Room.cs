using System;
using System.Collections.Generic;

namespace ConferenceAPI.Core.Models
{
    public partial class Room
    {
        public Room()
        {
            Blocks = new HashSet<Block>();
            Exceptions = new HashSet<Exception>();
            Reservations = new HashSet<Reservation>();
            RoomDevices = new HashSet<RoomDevice>();
        }

        public int RoomNumber { get; set; }
        public int Seats { get; set; }
        public int Layout { get; set; }
        public string Name { get; set; }
        public byte? ExceptionInversion { get; set; }

        public virtual Layout LayoutNavigation { get; set; }
        public virtual ICollection<Block> Blocks { get; set; }
        public virtual ICollection<Exception> Exceptions { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<RoomDevice> RoomDevices { get; set; }
    }
}
