using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeCommunicatorAPI.DTO;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Repositories;
using OfficeCommunicatorAPI.Services;

namespace OfficeCommunicatorAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly UserRepository _userRepository;
        private readonly AuthHelper _authHelper;

        public UserController(IMapper mapper, OfficeDbContext dbContext, AuthHelper authHelper)
        {
            _mapper = mapper;
            _userRepository = new UserRepository(dbContext, mapper, authHelper);
            _authHelper = authHelper;
        }

        [Authorize]
        [HttpGet("get")]
        public async Task<UserDto?> Get()
        {
            bool result = int.TryParse(User.FindFirst("userId")?.Value, out var userId);
            if (!result) return null;
            
            User? user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return null;
            
            UserDto userDto = _mapper.Map<UserDto>(user);
            return userDto;
        }
        
        
        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _userRepository.GetAllAsync());
        }

        
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserDto userDto)
        {
            if(!_authHelper.IsEmail(userDto.Email)) return BadRequest("Invalid email");
            if(_authHelper.IsEmail(userDto.UniqueName)) return BadRequest("Invalid nickname");
            if(userDto.Password.Length < 8) return BadRequest("Password must be at least 8 characters long");
            User user = await _userRepository.AddAsync(userDto);
            if (user.Id <= 0) return BadRequest("Failed to add user");
            
            string token = _authHelper.CreateToken(user);
            return Ok(token);
        }
        
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserEmailPasswordDto userDto)
        {
            Console.WriteLine("Login");
            User? user = await _userRepository.GetByEmailAsync(userDto);
            if (user == null) return BadRequest("User not found");
            
            string token = _authHelper.CreateToken(user);
            return Ok(new Dictionary<string, string>() { { "token", token } });
            //return Ok(token);
        }
        
        
        [Authorize]
        [HttpPost("update")]
        public async Task<IActionResult> Update(UserUpdateDto userDto)
        {
            bool result = int.TryParse(User.FindFirst("userId")?.Value, out var userId);
            if (!result) return BadRequest("Invalid user id");
            
            userDto.Id = userId;
            bool success = await _userRepository.UpdateAsync(userDto);
            return success ? Ok("User updated") : BadRequest("Nothing to update");
        }
    }
}