using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models {
    public class Story : BaseEntity {
        [Key]
        public int StoryId {  get; set; }
        public long UserId {  get; set; }
        [ForeignKey("UserId")]
        public User User {  get; set; }
        public string Type { get; set; }
        public string Path {  get; set; }
    }
}
