using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeCommunicatorMaui.Models
{
    public class MessageSecretStorageModel
    {
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public int DictionaryIndex { get; set; }
        public string Content { get; set; }

        public string Communication { get; set; }

        public MessageSecretStorageModel(int userId, int chatId, string content, string communication)
        {
            UserId = userId;
            ChatId = chatId;
            Content = content;
            Communication = communication;
        }
    }
}
