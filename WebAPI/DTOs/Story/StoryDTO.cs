using System.ComponentModel.DataAnnotations.Schema;
using WebAPI.Models;

namespace WebAPI.DTOs.Story
{
    public class StoryDTO
    {
        public int StoryId { get; set; }
        public string Type { get; set; }
        public string Path { get; set; }
    }
}
