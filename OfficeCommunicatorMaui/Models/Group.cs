namespace OfficeCommunicatorMaui.Models
{
    public class Group
    {
        public int Id { get; set; }
        public int UnviewedMessages { get; set; }
        public string Name { get; set; }
        public string UniqueIdentifier { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public List<User> Admins { get; set; }
        public List<User> Users { get; set; }
        public DateTime? DateTime { get; set; }
    }
}
