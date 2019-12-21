using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using ConferenceAPI.Core;
using ConferenceAPI.Core.Models;
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
    }
}