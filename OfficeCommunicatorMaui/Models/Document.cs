using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeCommunicatorMaui.Models
{
    public class Document
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public string Name { get; set; }
        public string UniqueIdentifier { get; set; }
    }
}
