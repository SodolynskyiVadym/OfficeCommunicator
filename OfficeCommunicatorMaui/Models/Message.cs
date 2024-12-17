namespace OfficeCommunicatorMaui.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public List<Document> Documents { get; set; } = new();
        public User User { get; set; }
    }
}
