using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceAPI.Core.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public Room Room { get; set; }
        public User User { get; set; }
        public Segment Segment { get; set; }


    }
}
