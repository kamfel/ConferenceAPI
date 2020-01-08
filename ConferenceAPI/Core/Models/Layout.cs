using System;
using System.Collections.Generic;

namespace ConferenceAPI.Core.Models
{
    public partial class Layout
    {
        public Layout()
        {
            Rooms = new HashSet<Room>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Room> Rooms { get; set; }
    }
}
