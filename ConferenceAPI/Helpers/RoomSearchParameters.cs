using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceAPI.Helpers
{
    public class RoomSearchParameters
    {
        public int MaxSeats { get; set; }
        public int MinSeats { get; set; }
        public string Layouts { get; set; }
        public string Devices { get; set; }

        public bool IsEmpty()
        {
            return MaxSeats == 0 && MinSeats == 0 && Layouts == null && Devices == null;
        }
    }
}
