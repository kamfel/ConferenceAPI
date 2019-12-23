using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceAPI.Core.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int AmountOfSeats { get; set; }
        public int RoomNumber { get; set; }
        public string Layout { get; set; }
        public string Name { get; set; }
        public IEnumerable<Reservation> Reservations { get; set; }
    }
}
