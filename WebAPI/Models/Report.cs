using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models {
    public class Report : BaseEntity {
        [Key]
        public long ReportId { get; set; }
        public long ReporterId { get; set; }
        [ForeignKey("ReporterId")]
        public User Reporter { get; set; }
        public long ReportedUserId { get; set; }
        [ForeignKey("ReportedUserId")]
        public User ReportedUser { get; set; }
    }

}
