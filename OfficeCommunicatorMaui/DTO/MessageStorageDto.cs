using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.AspNetCore.Components.Forms;
namespace OfficeCommunicatorMaui.DTO
{
    public class MessageStorageDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; }

        [NotMapped]
        public List<IBrowserFile> Files { get; set; } = new();

        public MessageStorageDto(int userId, int chatId, string content, List<IBrowserFile> files)
        {
            UserId = userId;
            ChatId = chatId;
            Content = content;
            Files = files;
        }

        public MessageStorageDto()
        {
        }
    }
}
