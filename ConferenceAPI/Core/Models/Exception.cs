using System;
using System.Collections.Generic;

namespace ConferenceAPI.Core.Models
{
    public partial class Exception
    {
        public int Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public int RoomNumber { get; set; }

        public virtual Room RoomNumberNavigation { get; set; }
    }
}
