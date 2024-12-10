namespace OfficeCommunicatorAPI.Models;

public class Contact
{
    public int Id { get; set; }
    public int ChatId { get; set; }
    public Chat Chat { get; set; }
    public int UserId { get; set; }
    public int AssociatedUserId { get; set; }
    public User AssociatedUser { get; set; }
}