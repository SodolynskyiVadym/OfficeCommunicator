﻿using OfficeCommunicatorAPI.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using OfficeCommunicatorAPI.DTO;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using OfficeCommunicatorAPI.Services.Checkers;

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
    private readonly GroupChecker _groupChecker;
    private readonly ContactChecker _contactChecker;

    public ChatController(IMapper mapper, OfficeDbContext dbContext, AuthHelper authHelper, BlobStorageService fileService)
    {
        _mapper = mapper;
        _userRepository = new UserRepository(dbContext, mapper, authHelper);
        _contactRepository = new ContactRepository(dbContext, mapper);
        _groupRepository = new GroupRepository(dbContext, mapper);
        _messageRepository = new MessageRepository(dbContext, mapper);
        _documentRepository = new DocumentRepository(dbContext);
        _fileService = fileService;
        _groupChecker = new GroupChecker(dbContext);
        _contactChecker = new ContactChecker(dbContext);
    }


    [Authorize]
    [HttpGet("get-contact/{associatedUserId}")]
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

        Group? group = await _groupRepository.GetGroupWithUsersAdmins(groupId, userId);
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
        Console.WriteLine($"Messege sent with content {messageDto.Content} and {files.Count()} files");


        if (!int.TryParse(User.FindFirst("userId")?.Value, out var userId)) return BadRequest("Invalid user id");
        messageDto.UserId = userId;

        Message? message;

        if(messageDto.CommunicationType.Equals(nameof(Group))) message = await _messageRepository.AddAsync(messageDto, _groupChecker);
        else if(messageDto.CommunicationType.Equals(nameof(Contact))) message = await _messageRepository.AddAsync(messageDto, _contactChecker);
        else return BadRequest("Invalid communication type");

        if (message == null) return BadRequest("Invalid message");

        string uniqueFileName;
        List<Document> documents = new();
        foreach (var file in files)
        {
            uniqueFileName = _fileService.GenerateFileName(file.FileName, message.Id);
            await _fileService.UploadFileAsync(file, uniqueFileName);
            documents.Add(new Document { MessageId = message.Id, Name = file.FileName, UniqueIdentifier = uniqueFileName });
        }
        documents = await _documentRepository.AddRangeAsync(documents);
        message.Documents = documents;
        return Ok(message);
    }


    [Authorize]
    [HttpPost("update-message")]
    public async Task<IActionResult> UpdateMessageAsync(MessageUpdateDto messageDto)
    {
        if (!int.TryParse(User.FindFirst("userId")?.Value, out var userId)) return BadRequest("Invalid user id");

        Message? message = await _messageRepository.UpdateAsync(messageDto);
        if (message == null) return BadRequest("Message wasn't updated");


        return Ok(message);
    }


    [Authorize]
    [HttpDelete("delete-message/{messageId}")]
    public async Task<IActionResult> DeleteMessage(int messageId)
    {
        if (!int.TryParse(User.FindFirst("userId")?.Value, out var userId)) return BadRequest("Invalid user id");
        Message? message = await _messageRepository.RemoveAsync(messageId, userId);
        if (message == null) return BadRequest("Message wasn't deleted");

        foreach (var document in message.Documents) await _fileService.DeleteFileAsync(document.UniqueIdentifier);
        return Ok(true);
    }


    [Authorize]
    [HttpDelete("delete-document/{documentId}")]
    public async Task<IActionResult> DeleteDocument(int documentId)
    {
        Console.WriteLine($"Document id for deleting {documentId}");
        if (!int.TryParse(User.FindFirst("userId")?.Value, out var userId)) return BadRequest("Invalid user id");

        Document? document = await _messageRepository.GetDocumentByIdAsync(documentId);
        if (document == null) return BadRequest("Document wasn't deleted");

        Message? message = await _messageRepository.GetByIdAsync(document.MessageId);
        if (message == null) return BadRequest("Message wasn't found");
        if(message.UserId != userId) return BadRequest("You aren't the author of the message");


        bool result = await _messageRepository.RemoveDocumentAsync(documentId);
        if(result) await _fileService.DeleteFileAsync(document.UniqueIdentifier);
        return Ok(result);
    }


    [Authorize]
    [HttpGet("download/{messegeId}/{documentId}")]
    public async Task<IActionResult> DownloadFile(int messegeId, int documentId)
    {
        Document? document = await _documentRepository.GetByIdAsync(documentId);
        if (document == null) return BadRequest("File not found");
        if (document.MessageId != messegeId) return BadRequest("Invalid file id");
        Console.WriteLine($"Document id {document.Id}");


        Stream stream = await _fileService.DownloadFileAsync(document.UniqueIdentifier);
        return File(stream, "application/octet-stream", document.Name);
    }


    [Authorize]
    [HttpPost("add-user-to-group")]
    public async Task<IActionResult> AddUserToGroup(UserGroupDto userGroupDto)
    {
        if (!int.TryParse(User.FindFirst("userId")?.Value, out var userId)) return BadRequest("Invalid user id");


        Console.WriteLine($"Group id {userGroupDto.GroupId} and user id {userGroupDto.UserId}");
        Console.WriteLine($"Sender is {userId}");
        User? result = await _groupRepository.AddUserToGroupAsync(userGroupDto.GroupId, userGroupDto.UserId, userId);
        if (result == null) return BadRequest("User wasn't added to the group");
        return Ok(result);
    }



    [Authorize]
    [HttpPost("add-admin")]
    public async Task<IActionResult> AddAdmin(UserGroupDto userGroupDto)
    {
        Console.WriteLine("Methid add admin work");
        if (!int.TryParse(User.FindFirst("userId")?.Value, out var userId)) return BadRequest("Invalid user id");
        bool result = await _groupRepository.AddAdminAsync(userGroupDto.GroupId, userGroupDto.UserId, userId);
        if(!result) return BadRequest("Admin wasn't added to the group");
        return Ok();
    }


    [Authorize]
    [HttpDelete("leave-group/{groupId}")]
    public async Task<IActionResult> LeaveGroup(int groupId)
    {
        if (!int.TryParse(User.FindFirst("userId")?.Value, out var userId)) return BadRequest("Invalid user id");

        bool result = await _groupRepository.RemoveUserFromGroupAsync(groupId, userId);
        if (!result) return BadRequest("User wasn't removed from the group");
        return Ok();
    }


    [Authorize]
    [HttpDelete("remove-user-from-group/{groupId}/{userId}")]
    public async Task<IActionResult> RemoveUser(int groupId, int userId)
    {
        if (!int.TryParse(User.FindFirst("userId")?.Value, out var adminUserId)) return BadRequest("Invalid user id");

        Console.WriteLine($"Method remove user from group work. Removing user {userId} from group {groupId}");

        bool result = await _groupRepository.RemoveUserFromGroupAsAdminAsync(groupId, userId, adminUserId);
        if (!result) return BadRequest("User wasn't removed from the group");
        return Ok();
    }


    [Authorize]
    [HttpDelete("delete-contact/{chatId}")]
    public async Task<IActionResult> RemoveContact(int chatId)
    {
        if (!int.TryParse(User.FindFirst("userId")?.Value, out var userId)) return BadRequest("Invalid user id");

        bool result = await _contactRepository.DeleteAsync(chatId, userId);
        if (!result) return BadRequest("Contact wasn't removed");
        return Ok();
    }

}