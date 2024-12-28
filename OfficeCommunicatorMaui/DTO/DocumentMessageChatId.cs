using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeCommunicatorMaui.DTO
{
    public class DocumentMessageChatId
    {
        public int DocumentId { get; set; }
        public int MessageId { get; set; }
        public int ChatId { get; set; }

        public DocumentMessageChatId(int documentId, int messageId, int chatId)
        {
            DocumentId = documentId;
            MessageId = messageId;
            ChatId = chatId;
        }
    }
}
