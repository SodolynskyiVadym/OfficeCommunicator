using OfficeCommunicatorAPI.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OfficeCommunicatorAPI.DTO;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace OfficeCommunicatorAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class ChatController : ControllerBase
{
    private readonly UserRepository _userRepository;
    private readonly GroupRepository _groupRepository;
    private readonly ContactRepository _contactRepository;
    private readonly MessageRepository _messageRepository;
    private readonly DocumentRepository _documentRepository;
    private readonly IMapper _mapper;
    private readonly BlobStorageService _fileService;

    public ChatController(IMapper mapper, OfficeDbContext dbContext, AuthHelper authHelper, BlobStorageService fileService)
    {
        _mapper = mapper;
        _userRepository = new UserRepository(dbContext, mapper, authHelper);
        _contactRepository = new ContactRepository(dbContext, mapper);
        _groupRepository = new GroupRepository(dbContext, mapper);
        _messageRepository = new MessageRepository(dbContext, mapper);
        _documentRepository = new DocumentRepository(dbContext);
        _fileService = fileService;
    }


    [Authorize]
    [HttpGet("get-contact/{associateduserId}")]
    public async Task<IActionResult> GetContact(int associatedUserId)
    {
        bool result = int.TryParse(User.FindFirst("userId")?.Value, out var userId);
        if (!result) return BadRequest("Invalid user id");

        Contact? contact = await _contactRepository.GetByUserIdAndAssociatedUserIdWithIncludesAsync(userId, associatedUserId);
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


    [Authorize]
    [HttpPost("create-message")]
    public async Task<IActionResult> CreateMessage([FromForm]string messageDtoJson, [FromForm]List<IFormFile> files)
    {
        MessageDto? messageDto = JsonConvert.DeserializeObject<MessageDto>(messageDtoJson);

        if (messageDto == null) return BadRequest("Invalid message");
        if (!int.TryParse(User.FindFirst("userId")?.Value, out var userId)) return BadRequest("Invalid user id");
        messageDto.UserId = userId;

        Message? message = await _messageRepository.AddAsync(messageDto);
        if (message == null) return BadRequest("Invalid message");

        string uniqueFileName;
        List<Document> documents = new();
        foreach (var file in files)
        {
            Console.WriteLine($"name {file.Name} file name is {file.FileName} content type is {file.ContentType}");
            uniqueFileName = _fileService.GenerateFileName(file.FileName, message.Id);
            await _fileService.UploadFileAsync(file, uniqueFileName);
            documents.Add(new Document { MessageId = message.Id, Name = file.FileName, UniqueIdentifier = uniqueFileName });
        }
        documents = await _documentRepository.AddRangeAsync(documents);
        message.Documents = documents;
        return Ok(message);
    }

    [Authorize]
    [HttpGet("download/{messegeId}/{documentId}")]
    public async Task<IActionResult> DownloadFile(int messegeId, int documentId)
    {
        Document? document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return BadRequest("File not found");
        if (document.MessageId != messegeId) return BadRequest("Invalid file id");

        Stream stream = await _fileService.DownloadFileAsync(document.UniqueIdentifier);
        return File(stream, "application/octet-stream", document.Name);
    }


    //[Authorize]
    //[HttpPost("updload-file")]
    //public async Task<IActionResult> UploadFile(IFormFile file)
    //{
    //    if(file == null || file.Length == 0) return BadRequest("File is null");
    //    bool result = int.TryParse(User.FindFirst("userId")?.Value, out var userId);
    //    if (!result) return BadRequest("Invalid user id");


    //    string path = await _fileService.UploadFileAsync(file);
    //    return Ok(path);
    //}

}