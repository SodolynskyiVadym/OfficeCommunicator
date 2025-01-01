using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Components.Forms;
namespace OfficeCommunicatorMaui.Models
{
    public class MessageStorageModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string CommunicationType { get; set; }
        public string UniqueIdentifier { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; }

        [NotMapped]
        public List<IBrowserFile> Files { get; set; } = new();

        public MessageStorageModel(int userId, string uniqueIdentifier, int chatId, string content, string communication, List<IBrowserFile> files)
        {
            UserId = userId;
            UniqueIdentifier = uniqueIdentifier;
            ChatId = chatId;
            Content = content;
            CommunicationType = communication;
            Files = files;
        }

        public MessageStorageModel()
        {
        }
    }
}
