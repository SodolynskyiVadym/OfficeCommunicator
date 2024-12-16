using OfficeCommunicatorAPI.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OfficeCommunicatorAPI.DTO;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Services;
using Microsoft.AspNetCore.Authorization;

namespace OfficeCommunicatorAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly UserRepository _userRepository;
    private readonly GroupRepository _groupRepository;
    private readonly ContactRepository _contactRepository;
    private readonly IMapper _mapper;
    
    public ChatController(IMapper mapper, OfficeDbContext dbContext, AuthHelper authHelper)
    {
        _mapper = mapper;
        _userRepository = new UserRepository(dbContext, mapper, authHelper);
        _contactRepository = new ContactRepository(dbContext, mapper);
        _groupRepository = new GroupRepository(dbContext, mapper);
    }


    [Authorize]
    [HttpGet("get-contact/{associateduserId}")]
    public async Task<IActionResult> GetContact(int associatedUserId)
    {
        bool result = int.TryParse(User.FindFirst("userId")?.Value, out var userId);
        if (!result) return BadRequest("Invalid user id");

        Contact? contact = await _contactRepository.GetByUserIdAndAssociatedUserIdAsync(userId, associatedUserId);
        return Ok(contact);
    }


    [Authorize]
    [HttpGet("get-group/{groupId}")]
    public async Task<IActionResult> GetGroup(int groupId)
    {
        bool result = int.TryParse(User.FindFirst("userId")?.Value, out var userId);
        if (!result) return BadRequest("Invalid user id");

        Group? group = await _groupRepository.GetGroupWithUsers(groupId, userId);
        return Ok(group);
    }


    [Authorize]
    [HttpPost("create-contact")]
    public async Task<IActionResult> CreateContact(ContactDto contactDto)
    {
        bool result = int.TryParse(User.FindFirst("userId")?.Value, out var userId);
        if (!result) return BadRequest("Invalid user id");
        if(userId == contactDto.AssociatedUserId) return BadRequest("Cannot add yourself as a contact");

        contactDto.UserId = userId;
        Contact contact = await _contactRepository.AddAsync(contactDto);
        return Ok(contact);
    }

    [Authorize]
    [HttpPost("create-group")]
    public async Task<IActionResult> CreateGroup(GroupDto groupDto)
    {
        bool result = int.TryParse(User.FindFirst("userId")?.Value, out var userId);
        if (!result) return BadRequest("Invalid user id");

        groupDto.UserAuthorId = userId;
        Group group = await _groupRepository.AddAsync(groupDto);
        return Ok(group);
    }

}