using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Constants;
using WebAPI.DTOs.Chats;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services {
    public class MessageService : BaseService<Message, MessageRepository> {
        private readonly UserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ChatMemberRepository _chatMemberRepository;
        private readonly IUrlHelper _urlHelper;
        public MessageService(MessageRepository repository, UserRepository userRepository, 
            IHttpContextAccessor httpContextAccessor, FileService fileService, 
            FileRepository fileRepository, ChatMemberRepository chatMemberRepository, 
            IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor) : base(repository) {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _chatMemberRepository = chatMemberRepository;

            var actionContext = new ActionContext(
                                    httpContextAccessor.HttpContext,
                                    httpContextAccessor.HttpContext.GetRouteData(),
                                    new ActionDescriptor());
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContext);
        }

        public async Task<MessageResponse> SaveMessageAsync(MessageRequest messageRequest) {
            var uuid = _httpContextAccessor.HttpContext?.User?.FindFirst(JwtType.UUID).Value;
            var user = await _userRepository.GetUserByUUIDAsync(uuid);
            if(user.ChatMembers == null) {
                return null;
            }
            var chatMember = user.ChatMembers.FirstOrDefault(cm => cm.Chat.UUID == messageRequest.ChatUUID);
            if(chatMember == null) {
                return null;
            }
            Message messSuccess = null;
            Message message = new Message() {
                UUID = Guid.NewGuid().ToString(),
                ChatMemberId = chatMember.ChatMemberId,
                Status = Status.ACTIVE,
                MessageFlags = new List<MessageFlag>()
            };
            if(messageRequest.MessageType == MessageType.TEXT) {
                message.Content = messageRequest.Content;
                message.ContentType = MessageType.TEXT;
                messSuccess = await _repository.AddAsync(message);

            } else if(messageRequest.MessageType == FileType.DOCUMENT 
                || messageRequest.MessageType == FileType.VIDEO 
                || messageRequest.MessageType == FileType.IMAGE) {
                message.Content = "";
                message.ContentType = MessageType.FILE;
                
                var listFile = new List<Models.File>();
                messageRequest.FileRequests.ForEach(async file => {
                    string extension = Path.GetExtension(file.Name);
                    string path = Guid.NewGuid() + extension;
                    string fileType = "";
                    if(messageRequest.MessageType == FileType.DOCUMENT) {
                        fileType = FileType.DOCUMENT;
                    }else if(messageRequest.MessageType == FileType.VIDEO) { 
                        fileType = FileType.VIDEO; 
                    }else if(messageRequest.MessageType == FileType.IMAGE) {
                        fileType = FileType.IMAGE; 
                    }
                    await FileService.SaveFileFromBase64Async(file.DataFile, path, fileType);
                    listFile.Add(new Models.File() {
                        Message = message,
                        Name = file.Name,
                        Path = path,
                        Type = fileType,
                        Status = Status.ACTIVE
                    });
                });
                message.Files = listFile;
                messSuccess = await _repository.ExecuteInTransactionAsync(async () => {
                     return await _repository.AddAsync(message);
                });
            }
            if(messSuccess == null) {
                return null;
            }

            var chatmember = await _chatMemberRepository.GetChatMemberByIdAsync(messSuccess.ChatMember.ChatMemberId);
            MessageResponse messageResponse = new MessageResponse() {
                MessageUUID = messSuccess.UUID,
                Content = messSuccess.Content,
                ContentType = messSuccess.ContentType,
                IsEdit = messSuccess.IsEdited,
                CreatedAt = messSuccess.CreatedAt,
                UpdatedAt = messSuccess.UpdatedAt,

                UserResponse = new UserResponse() {
                    AvatarUrl = _urlHelper.Action("GetImageInChat", "File", new { path = chatmember.User.Profile.AvatarUrl }, _httpContextAccessor?.HttpContext?.Request.Scheme),
                    NickName = chatMember.User.Profile.NickName,
                    UserUUID = chatMember.User.UUID
                }
            };
            if (messSuccess.Files.IsNullOrEmpty()) {
                return messageResponse;
            } else {
                var fileResponses = new List<FileResponse>();
                foreach (var file in messSuccess.Files)
                {
                    var path = "";
                    if(file.Type == FileType.DOCUMENT) {
                        path = _urlHelper.Action("DownLoadDocumentInchat", "File", new {path = file.Path}, _httpContextAccessor?.HttpContext?.Request.Scheme);
                    }else if(file.Type == FileType.IMAGE) {
                        path = _urlHelper.Action("GetImageInChat", "File", new { path = file.Path }, _httpContextAccessor?.HttpContext?.Request.Scheme);
                    } else if(file.Type == FileType.VIDEO) {
                        path = _urlHelper.Action("GetVideoInChat", "File", new { path = file.Path }, _httpContextAccessor?.HttpContext?.Request.Scheme);
                    }
                    fileResponses.Add(new FileResponse() {
                        Name = file.Name,
                        Path = path,
                        Type = file.Type
                    });
                }
                messageResponse.FileResponse = fileResponses;
            }
            return messageResponse;
        }

        public async Task<bool> DeleteMessageAsync(string messageUUID) {
            var message = await CheckMessageOfUser(messageUUID);
            if (message == null) return false;
            DateTime now = DateTime.Now;
            TimeSpan timeDifference = now - message.CreatedAt;
            if (timeDifference.TotalHours > 1) {
                return false;
            }
            message.Status = Status.INACTIVE;
            await _repository.UpdateAsync(message);
            return true;
        }

        public async Task<bool> EditMessageAsync(string contentText, string messageUUID, string chatUUID) {
            var message = await CheckMessageOfUser(messageUUID);
            if(message == null) return false;
            if (message.Content == contentText) {
                return true;
            }
            DateTime now = DateTime.Now;
            TimeSpan timeDifference = now - message.CreatedAt;
            if (timeDifference.TotalHours > 1) {
                return false;
            }
            message.Content = contentText;
            await _repository.UpdateAsync(message);
            return true;
        }
        private async Task<Message> CheckMessageOfUser(string messageUUID) {
            var uuid = _httpContextAccessor.HttpContext?.User?.FindFirst(JwtType.UUID).Value;
            if (uuid == null) {
                return null;
            }
            var message = await _repository.GetMessageByUUIDAsync(messageUUID);
            if (message == null) {
                return null;
            }
            if (message.ChatMember.User.UUID != uuid) {
                return null;
            }
            return message;
        }
    }
}
