using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeCommunicatorMaui.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
        public User User { get; set; }
    }
}
