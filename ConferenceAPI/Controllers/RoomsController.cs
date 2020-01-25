﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ConferenceAPI.Core;
using ConferenceAPI.Core.Models;
using ConferenceAPI.Core.Services;
using ConferenceAPI.DTO;
using ConferenceAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAvailabilityService _availabilityService;

        public RoomsController(IUnitOfWork unitOfWork, IMapper mapper, IAvailabilityService availabilityService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _availabilityService = availabilityService;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] RoomSearchParameters parameters)
        {
            if (!parameters.IsEmpty())
            {
                return Find(parameters);
            }

            var rooms = await _unitOfWork.GetRepository<Room>().GetAllAsync();
            var roomsDTO = _mapper.Map<ICollection<RoomDTO>>(rooms);

            return Ok(roomsDTO);
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetByIdAsync(int id, [FromQuery] string date, [FromQuery] string start, [FromQuery] string end)
        {
            var room = await _unitOfWork.GetRepository<Room>().GetByIdAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            if (date != null)
            {
                var d = DateTime.ParseExact(date, "ddMMyyyy", null);
                var timeFrames = _availabilityService.GetAvailableTimeFramesOnDate(room, d);
                return Ok(timeFrames);
            }
            if (start != null && end != null)
            {
                var s = DateTime.ParseExact(start, "ddMMyyyyhhmmss", null);
                var e = DateTime.ParseExact(end, "ddMMyyyyhhmmss", null);
                var timeFrames = _availabilityService.GetAvailableTimeFramesInRange(room, s, e);
                return Ok(timeFrames);
            }
            if (date != null || start != null || end != null)
            {
                return BadRequest("Invalid query parameters");
            }

            var roomDTO = _mapper.Map<RoomDetailsDTO>(room);

            return Ok(roomDTO);
        }

        [Authorize(Policy = "AdminOnly")]
        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(RoomDetailsDTO roomDTO)
        {
            var room = _mapper.Map<Room>(roomDTO);

            var layout = await _unitOfWork.GetRepository<Layout>().SingleOrDefaultAsync(l => l.Name == roomDTO.Layout) ?? new Layout() { Name = roomDTO.Layout };

            var roomDevices = new List<RoomDevice>();

            if (roomDTO.Devices != null && roomDTO.Devices.Any())
            {
                var devicesFromDB = await _unitOfWork.GetRepository<Device>().GetAllAsync();
                foreach (var deviceName in roomDTO.Devices)
                {
                    var device = devicesFromDB.SingleOrDefault(d => d.Name == deviceName) ?? new Device() { Name = deviceName };

                    roomDevices.Add(new RoomDevice() { Device = device, RoomNumberNavigation = room });
                }
            }

            room.LayoutNavigation = layout;
            room.RoomDevices = roomDevices;
            room.ExceptionInversion = roomDTO.ExceptionInversion ? (byte)1 : (byte)0;

            await _unitOfWork.GetRepository<Room>().AddAsync(room);
            await _unitOfWork.SaveChangesAsync();

            //TODO: Figure out what to return in body
            return Ok();
        }

        [Authorize(Policy = "AdminOnly")]
        [Route("{roomNumber:int}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveAsync(int roomNumber)
        {
            var room = await _unitOfWork.GetRepository<Room>().SingleOrDefaultAsync(r => r.RoomNumber == roomNumber);

            if (room != null)
            {
                _unitOfWork.GetRepository<Room>().Remove(room);
                await _unitOfWork.SaveChangesAsync();
            }

            return NoContent();
        }

        [Authorize(Policy = "AdminOnly")]
        [Route("{roomNumber:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(int roomNumber, [FromBody] RoomDetailsDTO roomDTO)
        {
            var room = await _unitOfWork.GetRepository<Room>().SingleOrDefaultAsync(r => r.RoomNumber == roomNumber);

            if (room != null)
            {
                _unitOfWork.GetRepository<Room>().Remove(room);
            }

            roomDTO.RoomNumber = roomNumber;

            return await CreateAsync(roomDTO);
        }

        [Authorize(Policy = "AdminOnly")]
        [Route("{roomId:int}/segments")]
        [HttpPost]
        public async Task<IActionResult> CreateSegmentAsync(int roomId, [FromBody] TimeFrameDTO timeFrame)
        {
            var segment = _mapper.Map<Block>(timeFrame);
            var room = await _unitOfWork.GetRepository<Room>().GetByIdAsync(roomId);
            room.Blocks.Add(segment);
            await _unitOfWork.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Policy = "AdminOnly")]
        [Route("{roomId:int}/segments")]
        [HttpDelete]
        public async Task<IActionResult> RemoveSegmentsInRange(int roomId, [FromQuery] string start, [FromQuery] string end)
        {
            Predicate<DateTime> isEarlierThanEndTime = dt => true;
            Predicate<DateTime> isLaterThanStartTime = dt => true;

            if (start != null)
            {
                var startTime = DateTime.ParseExact(start, "ddMMyyyyhhmmss", null);
                isLaterThanStartTime = dt => dt > startTime;
            }

            if (end != null)
            {
                var endTime = DateTime.ParseExact(end, "ddMMyyyyhhmmss", null);
                isEarlierThanEndTime = dt => dt < endTime;
            }

            var room = await _unitOfWork.GetRepository<Room>().GetByIdAsync(roomId);
            foreach (var segment in room.Blocks)
            {
                if (isEarlierThanEndTime(segment.StartTime) && isLaterThanStartTime(segment.EndTime))
                {
                    room.Blocks.Remove(segment);
                }
            }
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }

        private IActionResult Find(RoomSearchParameters parameters)
        {
            var layouts = parameters.Layouts.Split(',');
            var devices = parameters.Devices.Split(',');

            var rooms = _unitOfWork.GetRepository<Room>().Find(room =>
                            parameters.MinSeats < room.Seats &&
                            parameters.MaxSeats > room.Seats &&
                            layouts.Contains(room.LayoutNavigation.Name) &&
                            room.RoomDevices.Select(rd => rd.Device.Name).Any(d => devices.Contains(d)))
                            .ToList();

            var roomsDTO = _mapper.Map<IEnumerable<RoomDTO>>(rooms);

            return Ok(roomsDTO);
        }

    }
}