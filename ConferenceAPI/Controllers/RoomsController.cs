using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ConferenceAPI.Core;
using ConferenceAPI.Core.Models;
using ConferenceAPI.DTO;
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
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _unitOfWork.GetRepository<Room>().GetAllAsync();
            var roomsDTO = _mapper.Map<Room>(rooms);

            return Ok(roomsDTO);
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _unitOfWork.GetRepository<Room>().GetByIdAsync(id);

            return Ok(room);
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> Create(RoomDetailsDTO roomDTO)
        {
            var room = _mapper.Map<Room>(roomDTO);
            await _unitOfWork.GetRepository<Room>().AddAsync(room);
            await _unitOfWork.SaveChangesAsync();

            //TODO: Figure out what to return in body
            return Ok();
        }

        [Route("{number:int}")]
        [HttpDelete]
        public async Task<IActionResult> Remove(int roomNumber)
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
        public async Task<IActionResult> UpdateRoom(int roomNumber, [FromBody] RoomDetailsDTO roomDTO)
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
    }
}