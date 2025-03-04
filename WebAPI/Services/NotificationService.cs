using Microsoft.AspNetCore.SignalR;
using System.Security.Principal;
using System.Text.Json;
using WebAPI.Constants;
using WebAPI.DTOs.Notifications;
using WebAPI.Hubs;
using WebAPI.Models;
using WebAPI.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WebAPI.Services {
    public class NotificationService : BaseService<Notification, NotificationRepository> {
        private readonly UserRepository _userRepository;
        private readonly IHubContext<UserHub> _userHubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserNotificationRepository _userNotificationRepository;
        public NotificationService(NotificationRepository repository, UserRepository userRepository, IHubContext<UserHub> userHubContext, IHttpContextAccessor httpContextAccessor, UserNotificationRepository userNotificationRepository) : base(repository) {
            _userRepository = userRepository;
            _userHubContext = userHubContext;
            _httpContextAccessor = httpContextAccessor;
            _userNotificationRepository = userNotificationRepository;
        }

        public async Task<List<NotificationResponse>> LoadNotificationAsync(User user) {
            var notifications = new List<NotificationResponse>();
            try {
                notifications = user.UserNotifications
                    .Where(un => un.UpdatedAt >= DateTime.UtcNow.AddDays(-3))
                    .OrderByDescending(un => un.UpdatedAt)
                    .Select(un => new NotificationResponse() {
                        NotificationId = un.NotificationId,
                        Title = un.Notification.Title,
                        Type = un.Notification.Type,
                        Status = un.Status,
                        NotificationFriendResponse = JsonSerializer.Deserialize<NotificationFriendResponse>(un.Notification.Message)
                    }).ToList();
            } catch (Exception ex) {
                await Console.Out.WriteLineAsync(ex.Message);
            }
            return notifications;
        }

        public async Task AddNotificationAsync(string userUUID, string uuidF) {
            var user = await _userRepository.GetUserInforByUUID(userUUID);
            var userF = await _userRepository.GetUserInforByUUID(uuidF);
            Notification notification = new Notification() {
                Title = "Add Friend",
                Type = NotificationType.ADD_FRIEND,
                Status = Status.ACTIVE,
                Message = JsonSerializer.Serialize(new NotificationFriendResponse() {
                    Name = user.Profile.NickName,
                    AvatarUrl = user.Profile.AvatarUrl,
                    Content = "Friend request sent",
                    DateAgo = DateTime.UtcNow,
                    UserSendRequest = uuidF
                })
            };
            notification = await _repository.AddAsync(notification);
            UserNotification userNotification = new UserNotification() {
                UserId = userF.UserId,
                NotificationId = notification.NotificationId,
                Status = Status.UNREAD
            };
            await _userNotificationRepository.AddAsync(userNotification);
            await SendNotificationToUser(notification, uuidF);
        }
        private async Task SendNotificationToUser(Notification notification, string uuidF) {
            try {
                var connectionId = UserHub.UsersOnline.FirstOrDefault(pair => pair.Value.UUID == uuidF).Key;
                if (connectionId != null) {
                    NotificationResponse notificationResponse = new NotificationResponse() {
                        Title = notification.Title,
                        Status = Status.UNREAD,
                        Type = notification.Type,
                        NotificationId = notification.NotificationId,
                        NotificationFriendResponse = JsonSerializer.Deserialize<NotificationFriendResponse>(notification.Message)
                    };
                    await _userHubContext.Clients.Client(connectionId).SendAsync("ReceiveNotifications", notificationResponse);
                }
            } catch(Exception e) {
                await Console.Out.WriteLineAsync(e.Message);
            }
        }

        public async Task AddNotificationAsync(string uuidU, string uuidF, int code) {
            var user = await _userRepository.GetUserInforByUUID(uuidU);
            var userF = await _userRepository.GetUserInforByUUID(uuidF);
            var notificationFriendResponse = new NotificationFriendResponse() {
                Name = user.Profile.NickName,
                AvatarUrl = user.Profile.AvatarUrl,
                DateAgo = DateTime.UtcNow,
                UserSendRequest = uuidF
            };
            Notification notification = new Notification() {
                Status = Status.ACTIVE,
                Type = NotificationType.FRIEND_RESPONSE,
            };
            bool created = false;
            if (code == 1) {
                notification.Title = "Accepted";
                notificationFriendResponse.Content = "Accept to make friends";
                notification.Message = JsonSerializer.Serialize(notificationFriendResponse);
                created = true;
            } else if(await _repository.IsFriend(uuidU, uuidF)) {
                notification.Title = "Rejected";
                notificationFriendResponse.Content = "Unfriended you";
                notification.Message = JsonSerializer.Serialize(notificationFriendResponse);
                created = true;
            }
            if (created) {
                notification = await _repository.AddAsync(notification);
                UserNotification userNotification = new UserNotification() {
                    UserId = userF.UserId,
                    NotificationId = notification.NotificationId,
                    Status = Status.UNREAD
                };
                await _userNotificationRepository.AddAsync(userNotification);
                await SendNotificationToUser(notification, uuidF);
            }
        }
    }
}
