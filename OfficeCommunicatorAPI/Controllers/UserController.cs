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
        private readonly ACService _acService;

        public UserController(IMapper mapper, OfficeDbContext dbContext, AuthHelper authHelper, ACService acService)
        {
            _mapper = mapper;
            _userRepository = new UserRepository(dbContext, mapper, authHelper);
            _authHelper = authHelper;
            _acService = acService;
        }

        [Authorize]
        [HttpGet("get")]
        public async Task<User?> Get()
        {
            bool result = int.TryParse(User.FindFirst("userId")?.Value, out var userId);
            if (!result) return null;
            User? user = await _userRepository.GetByIdWithIncludeAsync(userId);
            user?.HideSensitiveData();
            return user;
        }


        [Authorize]
        [HttpGet("getUserGroupsContacts")]
        public async Task<IActionResult> GetUserGroupsContacts()
        {
            bool result = int.TryParse(User.FindFirst("userId")?.Value, out var userId);
            if (!result) return BadRequest("Invalid user id");

            User? user = await _userRepository.GetByIdWithIncludeAsync(userId);
            if (user == null) return BadRequest("User not found");

            return Ok(new { user.Groups, user.Contacts });
        }


        [HttpGet("getAll")]
        public async Task<IActionResult> GetAll()
        {
            List<User> users = await _userRepository.GetAllAsync();
            foreach (var user in users) user.HideSensitiveData();
            return Ok(users);
        }

        
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(UserDto userDto)
        {
            if(!_authHelper.IsEmail(userDto.Email)) return BadRequest("Invalid email");
            if(_authHelper.IsEmail(userDto.UniqueName)) return BadRequest("Invalid nickname");
            if(userDto.Password.Length < 5) return BadRequest("Password must be at least 5 characters long");

            (userDto.AzureIdentity, userDto.AzureToken) = await _acService.CreateUserAsync();


            User user = await _userRepository.AddAsync(userDto);
            if (user.Id <= 0) return BadRequest("Failed to add user");
            
            string token = _authHelper.CreateToken(user);
            return Ok(new Dictionary<string, string>() { { "token", token } });
        }
        
        
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserEmailPasswordDto userDto)
        {
            User? user = await _userRepository.GetByEmailAsync(userDto);
            if (user == null) return BadRequest("User not found");

            if (!_authHelper.CheckPassword(userDto.Password, user.PasswordHash, user.PasswordSalt)) return BadRequest("Invalid password");

            user.AzureToken = await _acService.UpdateToken(user.AzureIdentity);
            await _userRepository.UpdateAsync(_mapper.Map<UserUpdateDto>(user));

            string token = _authHelper.CreateToken(user);
            return Ok(new Dictionary<string, string>() { { "token", token } });
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