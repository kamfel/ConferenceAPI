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

        [HttpGet("~/")]
        public async Task<IActionResult> GetAll()
        {
            var rooms = await _unitOfWork.GetRepository<Room>().GetAllAsync();
            var roomsDTO = _mapper.Map<Room>(rooms);

            return Ok(roomsDTO);
        }

        [HttpGet("/{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var room = await _unitOfWork.GetRepository<Room>().GetByIdAsync(id);

            return Ok(room);
        }

        [HttpPost("~/")]
        public async Task<IActionResult> CreateRoom(RoomDetailsDTO roomDTO)
        {
            var room = _mapper.Map<Room>(roomDTO);
            await _unitOfWork.GetRepository<Room>().AddAsync(room);
            await _unitOfWork.SaveChangesAsync();

            //TODO: Figure out what to return in body
            return Ok();
        }

        [HttpDelete("~/{number:int}")]
        public async Task<IActionResult> DeleteRoom(int roomNumber)
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
    }
}