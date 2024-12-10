using OfficeCommunicatorAPI.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OfficeCommunicatorAPI.DTO;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Services;

namespace OfficeCommunicatorAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly GroupRepository _groupRepository;
    private readonly ContactRepository _contactRepository;
    private readonly IMapper _mapper;
    
    public ChatController(IMapper mapper, OfficeDbContext dbContext)
    {
        _mapper = mapper;
        _contactRepository = new ContactRepository(mapper, dbContext);
        _groupRepository = new GroupRepository(mapper, dbContext);
    }
    
    [HttpPost("create-contact")]
    public async Task<IActionResult> CreateContact(ContactDto contactDto)
    {
        Contact contact = await _contactRepository.AddAsync(contactDto);
        return Ok(contact);
    }
    
}