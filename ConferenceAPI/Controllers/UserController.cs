using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConferenceAPI.Core;
using ConferenceAPI.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using ConferenceAPI.Core.Models;
using AutoMapper;
using JWT.Builder;
using JWT.Algorithms;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace ConferenceAPI.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserController(IUnitOfWork unitOfWork,
                              IMapper mapper,
                              IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [Route("login")]
        [HttpPost]
        public async Task<IActionResult> LogIn([FromBody] UserAuthDTO user)
        {
            var userFromDB = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(u => u.Email == user.Email);

            if (userFromDB == null || !BCrypt.Net.BCrypt.Verify(user.Password, userFromDB.Password))
            {
                return BadRequest("Wrong email or password.");
            }

            var isUserAdmin = userFromDB.UserPermissions.Select(up => up.Permission).Any(p => p.Name == "admin");

            var token = new JwtBuilder()
                .WithAlgorithm(new HMACSHA384Algorithm())
                .WithSecret(_configuration["Secret"])
                .AddClaim("exp", DateTimeOffset.UtcNow.AddDays(7).ToUnixTimeSeconds())
                .AddClaim("admin", isUserAdmin)
                .AddClaim("sub", userFromDB.Id)
                .Build();

            return Ok(token);
        }

        [AllowAnonymous]
        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userDTO)
        {
            var userFromDB = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(u => u.Email == userDTO.Email);

            if (userFromDB != null)
            {
                return BadRequest("Email already taken.");
            }

            var user = _mapper.Map<User>(userDTO);
            user.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);

            await _unitOfWork.GetRepository<User>().AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return Ok();
        }

        [Route("users")]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var users = await _unitOfWork.GetRepository<User>().GetAllAsync();
            var usersDTO = _mapper.Map<IEnumerable<UserDTO>>(users);

            return Ok(usersDTO);
        }

        [Route("users/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var userDTO = _mapper.Map<UserInfoDTO>(user);

            return Ok(userDTO);
        }

        [Route("users/{id:int}")]
        [HttpDelete]
        public async Task<IActionResult> RemoveAsync(int id)
        {
            var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _unitOfWork.GetRepository<User>().Remove(user);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }
}