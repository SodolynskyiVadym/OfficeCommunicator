using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Components.Forms;
namespace OfficeCommunicatorMaui.DTO
{
    public class MessageStorageDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CommunicationType { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; }

        [NotMapped]
        public List<IBrowserFile> Files { get; set; } = new();

        public MessageStorageDto(int userId, string communicationType, int chatId, string content, List<IBrowserFile> files)
        {
            Id = id;
            UserId = userId;
            CommunicationType = communicationType;
            ChatId = chatId;
            Content = content;
            Files = files;
        }

        public MessageStorageDto()
        {
        }
    }
}
