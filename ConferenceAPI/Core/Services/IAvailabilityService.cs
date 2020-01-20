using ConferenceAPI.Core.Models;
using ConferenceAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceAPI.Core.Services
{
    public interface IAvailabilityService
    {
        public bool IsReservationPossible(Room room, TimeFrameDTO timeFrame);
        public ICollection<TimeFrameDTO> GetAvailableTimeFramesOnDate(Room room, DateTime date);
        public ICollection<TimeFrameDTO> GetAvailableTimeFramesInRange(Room room, DateTime start, DateTime end);
    }
}
