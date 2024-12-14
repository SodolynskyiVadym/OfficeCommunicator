using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeCommunicatorMaui.Models
{
    public class Chat
    {
        public int Id { get; set; }
        public List<Message> Messages { get; set; }
    }
}
