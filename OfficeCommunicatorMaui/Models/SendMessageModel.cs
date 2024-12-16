using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeCommunicatorMaui.Models
{
    public class SendMessageModel
    {
        public SendMessageModel(int chatId, string message)
        {
            ChatId = chatId;
            Content = message;
        }

        public int ChatId { get; set; }
        public string Content { get; set; }
    }
}
