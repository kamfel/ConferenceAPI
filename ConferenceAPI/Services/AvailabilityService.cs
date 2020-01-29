using ConferenceAPI.Core;
using ConferenceAPI.Core.Models;
using ConferenceAPI.Core.Services;
using ConferenceAPI.DTO;
using ConferenceAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConferenceAPI.Services
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AvailabilityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public ICollection<TimeFrameDTO> GetAvailableTimeFramesInRange(Room room, DateTime start, DateTime end)
        {
            if (!room.ExceptionInversion.HasValue)
            {
                throw new MemberAccessException($"Room {room.RoomNumber} has no exception inversion defined.");
            }

            var reservations = _unitOfWork.GetRepository<Reservation>().Find(r => r.RoomNumber == room.RoomNumber &&
                                                                                  r.Date >= start &&
                                                                                  r.Date <= end);
            var segments = _unitOfWork.GetRepository<Block>().Find(b => b.RoomNumber == room.RoomNumber &&
                                                                        b.StartTime >= start &&
                                                                        b.EndTime <= end);
            var exceptions = _unitOfWork.GetRepository<Core.Models.Exception>().Find(e => e.RoomNumber == room.RoomNumber &&
                                                                                          e.Start <= end &&
                                                                                          e.End >= start);

            return AnalyseForAvailibleTimeFrames(reservations, segments, exceptions, room.ExceptionInversion.Value != 0);
        }

        public ICollection<TimeFrameDTO> GetAvailableTimeFramesOnDate(Room room, DateTime date)
        {
            if (!room.ExceptionInversion.HasValue)
            {
                throw new MemberAccessException($"Room {room.RoomNumber} has no exception inversion defined.");
            }

            var reservations = _unitOfWork.GetRepository<Reservation>().Find(r => r.RoomNumber == room.RoomNumber &&
                                                                                          r.Date.Date == date).ToList();
            var segments = _unitOfWork.GetRepository<Block>().Find(b => b.RoomNumber == room.RoomNumber &&
                                                                        b.StartTime.Date == date).ToList();
            var exceptions = _unitOfWork.GetRepository<Core.Models.Exception>().Find(e => e.RoomNumber == room.RoomNumber &&
                                                                                          e.Start.Date <= date &&
                                                                                          e.End.Date >= date).ToList();

            return AnalyseForAvailibleTimeFrames(reservations, segments, exceptions, room.ExceptionInversion.Value != 0);
        }

        public bool IsReservationPossible(Room room, TimeFrameDTO timeFrame)
        {
            var available = GetAvailableTimeFramesInRange(room, timeFrame.Start, timeFrame.End);

            return available.Contains(timeFrame);
        }

        public ICollection<TimeFrameDTO> AnalyseForAvailibleTimeFrames(IEnumerable<Reservation> reservations, IEnumerable<Block> segments, IEnumerable<Core.Models.Exception> exceptions, bool isExceptionInversion)
        {
            var notReservedSegments = segments.Where(b => !reservations.Any(r => r.BlockId == b.Id)).ToList();

            if (isExceptionInversion == false)
            {
                foreach (var exception in exceptions)
                {
                    foreach (var segment in notReservedSegments)
                    {
                        if (exception.Start < segment.EndTime && exception.End > segment.StartTime)
                        {
                            notReservedSegments.Remove(segment);
                        }
                    }
                }
            }
            else
            {
                foreach (var exception in exceptions)
                {
                    foreach (var segment in notReservedSegments)
                    {
                        if (exception.Start > segment.StartTime || exception.End < segment.EndTime)
                        {
                            notReservedSegments.Remove(segment);
                        }
                    }
                }
            }

            return notReservedSegments.ConvertAll(b => new TimeFrameDTO() { Start = b.StartTime, End = b.EndTime });
        } 
    }
}
