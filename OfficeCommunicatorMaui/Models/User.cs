using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeCommunicatorMaui.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string UniqueName { get; set; }
        public List<Group> Groups { get; set; }
        public List<Contact> Contacts { get; set; }
    }
}
