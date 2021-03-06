﻿using System;
using System.Collections.Generic;

namespace ConferenceAPI.Core.Models
{
    public partial class Device
    {
        public Device()
        {
            RoomDevices = new HashSet<RoomDevice>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<RoomDevice> RoomDevices { get; set; }
    }
}
