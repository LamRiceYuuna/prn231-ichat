using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Models;

namespace WebAPI.DTOs.Files
{
    public class FileResponseDto
    {
        public long FileId { get; set; }
        public long MessageId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}