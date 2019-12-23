using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceAPI.DTO
{
    public class RoomDetailsDTO
    {
        public int Number { get; set; }
        public int Name { get; set; }
        public string Layout { get; set; }
        public int NumberOfSeats { get; set; }
        public IEnumerable<string> Devices { get; set; }
    }
}
