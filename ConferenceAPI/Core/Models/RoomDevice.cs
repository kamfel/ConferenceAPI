using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceAPI.Core.Models
{
    public class RoomDevice
    {
        public Room Room { get; set; }
        public Device Device { get; set; }
    }
}
