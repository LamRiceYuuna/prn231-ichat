using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAPI.Models {
    public class UserAuthProvider : BaseEntity{
        [Key]
        public long AuthProviderId {  get; set; }
        public long UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string AuthProvider {  get; set; }
        public string? ProviderId {  get; set; }
        
    }
}
