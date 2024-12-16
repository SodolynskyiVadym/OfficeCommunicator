using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeCommunicatorMaui.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public int UnviewedMessages { get; set; }
        public int ChatId { get; set; }
        public Chat Chat { get; set; }
        public int UserId { get; set; }
        public int AssociatedUserId { get; set; }
        public User AssociatedUser { get; set; }

        public override string? ToString()
        {
            return $"{AssociatedUser.Name} ({UnviewedMessages})";
        }
    }
}
