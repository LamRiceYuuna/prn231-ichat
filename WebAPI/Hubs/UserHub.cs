using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.RegularExpressions;
using WebAPI.DTOs.Chats;
using WebAPI.Models;
using WebAPI.Services;

namespace WebAPI.Hubs {
    [Authorize("User")]
    public class UserHub : Hub {
        public static readonly ConcurrentDictionary<string, User> UsersOnline = new ConcurrentDictionary<string, User>();
        private static readonly ConcurrentDictionary<string, int> GroupConnectionCounts = new ConcurrentDictionary<string, int>();
        private static readonly ConcurrentDictionary<string, Statistic> Statistics = new ConcurrentDictionary<string, Statistic>();
        public readonly UserService _userService;
        private readonly MessageService _messageService;
        private readonly UserNotificationService _notificationService;
        private readonly StatisticService _statisticService;
        public UserHub(UserService userService, MessageService messageService, UserNotificationService notificationService, StatisticService statisticService) {
            _userService = userService;
            _messageService = messageService;
            _notificationService = notificationService;
            _statisticService = statisticService;
        }

        public override async Task OnConnectedAsync() {
            var user = await _userService.GetCurrentUserAsync();
            UsersOnline.TryAdd(Context.ConnectionId, user);
            await UserJoinGroup(user);
            Statistics.AddOrUpdate(Context.ConnectionId, key => new Statistic {
                User = user,
                LoginAt = DateTime.Now,
            }, (key, existingStatistic) => {
                return existingStatistic;
            });
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception) {
            if (UsersOnline.TryRemove(Context.ConnectionId, out var user)) {
                await UserLeaveGroup(user);
            }
            Statistics.AddOrUpdate(Context.ConnectionId, key => new Statistic {
                User = user,
                LoginAt = DateTime.Now,
            }, (key, existingStatistic) => {
                existingStatistic.LogoutAt = DateTime.Now;
                return existingStatistic;
            });
            bool removed = Statistics.TryRemove(Context.ConnectionId, out var removedStatistic);
            await _statisticService.RecordTimeAsync(removedStatistic);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task UserJoinGroup(User user) {
            if (user.ChatMembers != null) {
                var tasks = user.ChatMembers.Select(async chatMember => {
                    string chatUUID = chatMember.Chat.UUID;
                    await Groups.AddToGroupAsync(Context.ConnectionId, chatUUID);
                    GroupConnectionCounts.AddOrUpdate(chatUUID, 1, (key, count) => count + 1);
                    if (!IsGroupEmpty(chatUUID)) {
                        await Clients.Group(chatUUID).SendAsync("OnStatusChat", chatUUID, true);
                    }
                });
                await Task.WhenAll(tasks);
            }
        }

        public async Task UserLeaveGroup(User user) {
            if (user.ChatMembers != null) {
                var tasks = user.ChatMembers.Select(async chatMember => {
                    string chatUUID = chatMember.Chat.UUID;
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatUUID);
                    GroupConnectionCounts.AddOrUpdate(chatUUID, 0, (key, count) => count > 0 ? count - 1 : 0);
                    if (IsGroupEmpty(chatUUID)) {
                        await Clients.Group(chatUUID).SendAsync("OnStatusChat", chatUUID, false);
                    }
                });
                await Task.WhenAll(tasks);
            }
        }
        public async Task<MessageResponse> SendDataToChat(MessageRequest messageRequest) {
            MessageResponse message = await _messageService.SaveMessageAsync(messageRequest);
            await Clients.OthersInGroup(messageRequest.ChatUUID).SendAsync("LoadMessageReceiveChat", message, messageRequest.ChatUUID);
            return message;
        }

        public async Task<bool> EditMessageInChat(string contentText, string messageUUID, string chatUUID) {
            bool status = await _messageService.EditMessageAsync(contentText, messageUUID, chatUUID);
            if (status) {
                await Clients.OthersInGroup(chatUUID).SendAsync("LoadMessageEdit", contentText, messageUUID);
            }
            return status;
        }

        public async Task<bool> DeleteMessageInChat(string messageUUID, string chatUUID) {
            bool status = await _messageService.DeleteMessageAsync(messageUUID);
            if (status) {
                await Clients.OthersInGroup(chatUUID).SendAsync("LoadMessageDelete", messageUUID);
            }
            return status;
        }

        public async Task NotifyChatListUpdate(string userId)
        {
            await Clients.User(userId).SendAsync("UpdateChatList");
         }
         
        public bool IsGroupEmpty(string groupName) {
            return GroupConnectionCounts.TryGetValue(groupName, out int count) && count <= 1;
        }

        public async Task<bool> NoticeSeen(Dictionary<string, string> notifications) {
            bool status = await _notificationService.ReadNotificationAllAsync(notifications);
            return status;
        }
    }
}
