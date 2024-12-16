namespace OfficeCommunicatorMaui.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SqliteIndex { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public User User { get; set; }
    }
}
