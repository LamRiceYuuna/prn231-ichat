using AutoMapper;
using WebAPI.DTOs.Chats;
using WebAPI.DTOs.Friendships;
using WebAPI.DTOs.Story;
using WebAPI.DTOs.Users;
using WebAPI.Models;
using WebAPI.DTOs.ChatMembers;
using WebAPI.DTOs.Files;
using WebAPI.DTOs.Profiles;


namespace WebAPI.Mappers {
    public class TMapperProfile : AutoMapper.Profile
    {
        public TMapperProfile() {
            CreateMap<Models.User, UserDTO>()
            .ForMember(dest => dest.NickName, opt => opt.MapFrom(src => src.Profile.NickName))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Profile.AvatarUrl))
            .ForMember(dest => dest.Friendships, opt => opt.MapFrom(src =>
                src.Friendships.Where(f => f.FriendUser != null).Select(f => new FriendshipDTO
                {
                    UUID = f.FriendUser.UUID,
                    Username = f.FriendUser.UserName,
                    NickName = f.FriendUser.Profile.NickName,
                    AvatarUrl = f.FriendUser.Profile.AvatarUrl,
                    CreatedAt = f.CreatedAt
                }).ToList()))
            .ForMember(dest => dest.BlockedUsers, opt => opt.MapFrom(src =>
                src.BlockedUsers.Where(f => f.Blocked != null).Select(f => f.Blocked).Select(u => new BlockedUserDTO
                {
                    UUID = u.UUID,
                    Username = u.UserName,
                    NickName = u.Profile.NickName,
                    AvatarUrl = u.Profile.AvatarUrl
                }).ToList()));

            CreateMap<Models.Friendship, FriendshipDTO>();
            CreateMap<Models.BlockedUser, BlockedUserDTO>();
            CreateMap<Models.Story, StoryDTO>();    

            CreateMap<Models.User, UserInfoDTO>()
            .ForMember(dest => dest.NickName, opt => opt.MapFrom(src => src.Profile.NickName))
            .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Profile.AvatarUrl));

            CreateMap<Models.Chat, ChatResponse>()
                .ForMember(dest => dest.UUID, opt => opt.MapFrom(src => src.UUID))
                .ForMember(dest => dest.ChatName, opt => opt.MapFrom(src => src.ChatName))
                .ForMember(dest => dest.IsGroup, opt => opt.MapFrom(src => src.IsGroup))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.MessageResponse, opt => opt.MapFrom(src => src.ChatMembers.SelectMany(cm => cm.Messages).OrderByDescending(m => m.CreatedAt)));

            CreateMap<Models.Message, MessageResponse>()
                .ForMember(dest => dest.MessageUUID, opt => opt.MapFrom(src => src.UUID))
                .ForMember(dest => dest.UserResponse, opt => opt.MapFrom(src => src.ChatMember.User))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
                .ForMember(dest => dest.IsEdit, opt => opt.MapFrom(src => src.IsEdited))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.FileResponse, opt => opt.MapFrom(src => src.Files));

            CreateMap<Models.User, UserResponse>()
                .ForMember(dest => dest.UserUUID, opt => opt.MapFrom(src => src.UUID))
                .ForMember(dest => dest.NickName, opt => opt.MapFrom(src => src.Profile.NickName))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Profile.AvatarUrl));
                
            CreateMap<Models.File, FileResponse>()
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Path, opt => opt.MapFrom(src => src.Path))
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type));
                
            CreateMap<Chat, ChatResponseDTO>()
                .ForMember(dest => dest.UUID, opt => opt.MapFrom(src => src.UUID))
                .ForMember(dest => dest.ChatName, opt => opt.MapFrom(src => src.ChatName))
                .ForMember(dest => dest.IsGroup, opt => opt.MapFrom(src => src.IsGroup))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl))
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.LastMessage))
                .ForMember(dest => dest.LastMessageSentTime, opt => opt.MapFrom(src => src.LastMessageSentTime))
                .ForMember(dest => dest.LastMessageIsRead, opt => opt.MapFrom(src => src.LastMessageIsRead))
                .ForMember(dest => dest.OtherUser, opt => opt.MapFrom(src => src.OtherUser));

            CreateMap<Message, MessageDTO>()
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.SentTime, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.IsRead, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.MessageFlags.Any(f => f.ChatMember.UserId == (long)context.Items["UserId"] && f.Status == "Read")));

            CreateMap<User, UserResponseDTO>()
                .ForMember(dest => dest.UserUUID, opt => opt.MapFrom(src => src.UUID))
                .ForMember(dest => dest.NickName, opt => opt.MapFrom(src => src.Profile.NickName))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Profile.AvatarUrl));

            CreateMap<User, FriendDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UUID, opt => opt.MapFrom(src => src.UUID))
                .ForMember(dest => dest.NickName, opt => opt.MapFrom(src => src.Profile.NickName))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Profile.AvatarUrl));

            CreateMap<Models.File, FileResponseDto>();

            CreateMap<Models.ChatMember, ChatMemberResponse>();

            CreateMap<Models.Profile, ProfileDto>()
            .ForMember(dest => dest.Status, opt => opt.Ignore()) 
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) 
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }

}