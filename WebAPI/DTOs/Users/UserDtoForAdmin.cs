namespace WebAPI.DTOs.Users {
    public class UserDtoForAdmin {
        public string UUID {  get; set; }
        public string NickName {  get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed {  get; set; }
        public bool HasPassword {  get; set; }
        public DateTime LastLogin { get; set; }
        public string Role { get; set; }
        public string Status {  get; set; }
    }
}
