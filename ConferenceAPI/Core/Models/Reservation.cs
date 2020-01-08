using System;
using System.Collections.Generic;

namespace ConferenceAPI.Core.Models
{
    public partial class Reservation
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public int BlockId { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }

        public virtual Block Block { get; set; }
        public virtual Room RoomNumberNavigation { get; set; }
        public virtual User User { get; set; }
    }
}
