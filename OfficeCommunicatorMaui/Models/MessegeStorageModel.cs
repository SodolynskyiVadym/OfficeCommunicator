using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeCommunicatorMaui.Models
{
    public class MessegeStorageModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public string Content { get; set; }

        public string Communication { get; set; }

        public MessegeStorageModel(int userId, int chatId, string content, string communication)
        {
            UserId = userId;
            ChatId = chatId;
            Content = content;
            Communication = communication;
        }

        public MessegeStorageModel()
        {
        }
    }
}
