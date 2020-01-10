﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ConferenceAPI.Core;
using ConferenceAPI.Core.Models;
using ConferenceAPI.DTO;
using ConferenceAPI.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] RoomSearchParameters parameters)
        {
            if (!parameters.IsEmpty())
            {
                return FindAsync(parameters);
            }

            var rooms = await _unitOfWork.GetRepository<Room>().GetAllAsync();
            var roomsDTO = _mapper.Map<ICollection<RoomDTO>>(rooms);

            return Ok(roomsDTO);
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var room = await _unitOfWork.GetRepository<Room>().GetByIdAsync(id);

            return Ok(room);
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(RoomDetailsDTO roomDTO)
        {
            var room = _mapper.Map<Room>(roomDTO);
            await _unitOfWork.GetRepository<Room>().AddAsync(room);
            await _unitOfWork.SaveChangesAsync();

            //TODO: Figure out what to return in body
            return Ok();
        }

        [Route("{number:int}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveAsync(int roomNumber)
        {
            var rooms = _unitOfWork.GetRepository<Room>().Find(r => r.RoomNumber == roomNumber);

            var room = rooms.SingleOrDefault();

            if (room != null)
            {
                _unitOfWork.GetRepository<Room>().Remove(room);
                await _unitOfWork.SaveChangesAsync();
            }

            return NoContent();
        }

        [Route("{roomNumber:int}")]
        [HttpPut]
        public async Task<IActionResult> UpdateAsync(int roomNumber, [FromBody] RoomDetailsDTO roomDTO)
        {
            var rooms = _unitOfWork.GetRepository<Room>().Find(r => r.RoomNumber == roomNumber);

            var room = rooms.SingleOrDefault();

            if (room != null)
            {
                _unitOfWork.GetRepository<Room>().Remove(room);
            }

            var newRoom = _mapper.Map<Room>(roomDTO);

            await _unitOfWork.GetRepository<Room>().AddAsync(newRoom);

            await _unitOfWork.SaveChangesAsync();

            return Ok(roomDTO);
        }

        private IActionResult FindAsync([FromQuery] RoomSearchParameters parameters)
        {
            var layouts = parameters.Layouts.Split(',');
            var devices = parameters.Devices.Split(',');

            var rooms = _unitOfWork.GetRepository<Room>().Find(room => 
                            parameters.MinSeats < room.Seats &&
                            parameters.MaxSeats > room.Seats &&
                            layouts.Contains(room.LayoutNavigation.Name.ToLower()) &&
                            devices.Intersect(room.RoomDevices.Select(rd => rd.Device.Name.ToLower())).Count() != 0);

            var roomsDTO = _mapper.Map<IEnumerable<RoomDTO>>(rooms);

            return Ok(roomsDTO);
        }
    }
}