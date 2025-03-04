using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models
{
    public class Statistic : BaseEntity
    {
        [Key]
        public long StatisticId { get; set; }
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public DateTime LoginAt { get; set; }
        public DateTime LogoutAt { get; set; }

    }
}
