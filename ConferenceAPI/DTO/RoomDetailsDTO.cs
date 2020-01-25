using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ConferenceAPI.DTO
{
    public class RoomDetailsDTO
    {
        [Required]
        public int RoomNumber { get; set; }
        [Required]
        public int Seats { get; set; }
        [Required]
        public string Layout { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public bool ExceptionInversion { get; set; }

        public string[] Devices { get; set; }
    }
}
