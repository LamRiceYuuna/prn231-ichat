namespace WebAPI.DTOs.Statistics {
    public class StatisticDTO {
        public string UUID { get; set; }
        public string Username {  get; set; }
        public string Email {  get; set; }
        public DateTime LastLogin { get; set; }
        public string NickName {  get; set; }
        public string AvatarUrl {  get; set; }
        public TimeSpan AccessTime {  get; set; }
        public string AccessTimeFormatted => $"{(int)AccessTime.TotalHours}h {AccessTime.Minutes}m {AccessTime.Seconds}s";
    }
}
