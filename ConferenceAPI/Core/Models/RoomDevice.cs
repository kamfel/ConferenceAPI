﻿using System;
using System.Collections.Generic;

namespace ConferenceAPI.Core.Models
{
    public partial class RoomDevice
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int RoomNumber { get; set; }

        public virtual Device Device { get; set; }
        public virtual Room RoomNumberNavigation { get; set; }
    }
}
