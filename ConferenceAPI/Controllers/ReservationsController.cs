using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using ConferenceAPI.Core;
using ConferenceAPI.Core.Models;
using ConferenceAPI.Core.Services;
using ConferenceAPI.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ConferenceAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAvailabilityService _availabilityService;

        public ReservationsController(IUnitOfWork unitOfWork, 
                                      IMapper mapper,
                                      IAvailabilityService availabilityService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _availabilityService = availabilityService;
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var reservations = await _unitOfWork.GetRepository<Reservation>().GetAllAsync();
           
            if (User.FindFirst("admin").Value != "true")
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);

                var userId = int.Parse(claim.Value);

                reservations = reservations.Where(r => r.UserId == userId);
            }

            var reservationsDTO = _mapper.Map<IEnumerable<ReservationDTO>>(reservations);
            return Ok(reservationsDTO);
        }

        [HttpGet("api/users/{userId:int}/reservations")]
        public IActionResult GetAllForUser(int userId)
        {
            if (User.FindFirst("admin").Value != "true")
            {
                //Current user isn't admin
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);

                var userIdToken = int.Parse(claim.Value);

                if (userId != userIdToken)
                {
                    return BadRequest($"Cannot get other user's reservations");
                }
            }

            var reservations = _unitOfWork.GetRepository<Reservation>().Find(r => r.UserId == userId).ToList();
            var reservationsDTO = _mapper.Map<IEnumerable<ReservationDTO>>(reservations);

            return Ok(reservationsDTO);
        }

        [Route("{id:int}")]
        [HttpGet]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var reservation = await _unitOfWork.GetRepository<Reservation>().GetByIdAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            var reservationDTO = _mapper.Map<ReservationDTO>(reservation);

            return Ok(reservationDTO);
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] ReservationDTO reservationDTO)
        {
            var room = await _unitOfWork.GetRepository<Room>().SingleOrDefaultAsync(r => r.RoomNumber == reservationDTO.RoomNumber);

            if (room == null)
            {
                return BadRequest($"Room {reservationDTO.RoomNumber} doesn't exist.");
            }

            var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(u => u.Email == reservationDTO.UserEmail);

            if (user == null)
            {
                return BadRequest($"The email of user, who the reservation is made for, doesn't exist.");
            }

            if (User.FindFirst("admin").Value != "true")
            {
                //Current user isn't admin
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);

                var userId = int.Parse(claim.Value);

                if (user.Id != userId)
                {
                    return BadRequest($"Cannot make a reservation for a different user");
                }
            }

            var segment = await _unitOfWork.GetRepository<Block>().SingleOrDefaultAsync(b => b.RoomNumber == reservationDTO.RoomNumber &&
            b.StartTime == reservationDTO.StartingTime &&
            b.EndTime == reservationDTO.EndingTime);

            if (segment == null)
            {
                return BadRequest($"Reservation time bounds don't coincide with a valid segment of room {reservationDTO.RoomNumber}.");
            }

            if (segment.Reservations.Count() != 0)
            {
                //TODO: What status code?
                return BadRequest("The segment is already taken.");
            }

            var reservation = new Reservation()
            {
                RoomNumber = reservationDTO.RoomNumber,
                BlockId = segment.Id,
                UserId = user.Id,
                Date = reservationDTO.StartingTime.Date,
            };

            await _unitOfWork.GetRepository<Reservation>().AddAsync(reservation);
            await _unitOfWork.SaveChangesAsync();

            return Ok();
        }

        [Route("{id:int}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveAsync(int id)
        {
            var reservation = await _unitOfWork.GetRepository<Reservation>().GetByIdAsync(id);

            if (reservation != null)
            {
                _unitOfWork.GetRepository<Reservation>().Remove(reservation);
                await _unitOfWork.SaveChangesAsync();
            }

            return NoContent();
        }
    }
}