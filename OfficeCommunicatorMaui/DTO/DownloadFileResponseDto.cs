using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeCommunicatorMaui.DTO
{
    public class DownloadFileResponseDto
    {
        public byte[] File { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }

        public DownloadFileResponseDto(byte[] file, string fileName, string contentType)
        {
            File = file;
            FileName = fileName;
            ContentType = contentType;
        }
    }
}
