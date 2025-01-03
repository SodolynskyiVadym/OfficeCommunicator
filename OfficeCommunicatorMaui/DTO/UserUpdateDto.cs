using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeCommunicatorMaui.DTO
{
    public class UserUpdateDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? ZoomUrl { get; set; }
        public string? AzureToken { get; set; }
        public string? Password { get; set; }

        public UserUpdateDto(string name, string zoomUrl, string password)
        {
            Name = name;
            ZoomUrl = zoomUrl;
            Password = password;
        }
    }
}
