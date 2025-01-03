namespace OfficeCommunicatorMaui.Models
{
    public class DownloadFileRequestDto
    {
        public int DocumentId { get; set; }
        public int MessageId { get; set; }
        public string FileName { get; set; }

        public DownloadFileRequestDto(int documentId, int messageId, string fileName)
        {
            DocumentId = documentId;
            MessageId = messageId;
            FileName = fileName;
        }
    }
}
