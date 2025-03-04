using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models {
    public class File : BaseEntity{
        [Key]
        public long FileId {  get; set; }
        public long MessageId {  get; set; }
        [ForeignKey("MessageId")]
        public Message Message { get; set; }
        public string Name { get; set; }
        public string Path {  get; set; }
        public string Type {  get; set; }
    }
}
