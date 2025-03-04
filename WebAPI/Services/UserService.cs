using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Security.Claims;
using WebAPI.Authentication;
using WebAPI.Constants;
using WebAPI.DTOs.Statistics;
using WebAPI.DTOs.Users;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services {
    public class UserService : BaseService<User, UserRepository> {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JwtHandler _jwtHandler;
        private readonly RoleRepository _roleRepository;
        private readonly FileService _fileService;
        private readonly IUrlHelper _urlHelper;
        private readonly StatisticService _statisticService;
        public UserService(UserRepository repository, JwtHandler jwtHandler, IHttpContextAccessor httpContextAccessor,
            RoleRepository roleRepository, FileService fileService, IUrlHelperFactory urlHelperFactory, StatisticService statisticService) : base(repository) {
            _jwtHandler = jwtHandler;
            _httpContextAccessor = httpContextAccessor;
            _roleRepository = roleRepository;
            _fileService = fileService;
            _statisticService = statisticService;

            var actionContext = new ActionContext(
                                    httpContextAccessor.HttpContext,
                                    httpContextAccessor.HttpContext.GetRouteData(),
                                    new ActionDescriptor());
            _urlHelper = urlHelperFactory.GetUrlHelper(actionContext);
        }
        //Done
        public async Task<LoginResponse> Login(LoginRequest loginRequest) {
            var rs = new LoginResponse() {
                Status = false,
                Message = ""
            };
            var user = await _repository.GetUserByUsername(loginRequest.UserName);
            if (user == null) {
                rs.Message = "Username is incorrect!";
                return rs;
            }
            if(user.Status == Status.BANNED) {
                rs.Message = "The account has been disabled by the admin";
                return rs;
            }
            if (user.Password != loginRequest.Password) {
                rs.Message = "Password is incorrect!";
                return rs;
            }
            rs.Status = true;
            await UpdateLatLoginAsync(user);
            user.Profile.AvatarUrl = _urlHelper.Action("GetImageInChat", "File", new { path = user.Profile.AvatarUrl }, _httpContextAccessor?.HttpContext?.Request.Scheme);
            rs.Message = _jwtHandler.GenerateToken(user);
            return rs;
        }

        //Mẫu lấy ra user đăng nhập vào hệ thống
        public async Task<User> GetCurrentUserAsync() {
            var userUUID = _httpContextAccessor.HttpContext?.User?.FindFirst(JwtType.UUID)?.Value;
            var user = await _repository.GetUserByUUIDAsync(userUUID);
            return user;
        }

        public async Task<User> CreateUserFromGoooleClaimsAsync(IEnumerable<Claim> claims) {
            string? name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            string? email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string? nameidentifier = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            string? avatar = claims.FirstOrDefault(c => c.Type == "picture")?.Value;

            var user = await _repository.GetUserByEmailAsync(email);
            if (user != null) {
                return user;
            }

            string uuid = Guid.NewGuid().ToString();
            user = new User() {
                UUID = uuid,
                UserName = Faker.Internet.UserName(),
                Email = email,
                IsEmailConfirmed = true,
                HasPassword = false,
                Password = Guid.NewGuid().ToString(),
                LastLogin = DateTime.Now,
                Role = await _roleRepository.GetRoleByRoleName(RoleType.USER),
                Status = Status.ACTIVE,
                Profile = new Profile() {
                    User = user,
                    NickName = name,
                    AvatarUrl = avatar != null ? await _fileService.DownloadImageAsync(avatar) : "avatar_default.jpg",
                    Status = Status.ACTIVE
                },
                AuthProviders = new List<UserAuthProvider>() {
                    new UserAuthProvider() {
                        ProviderId = nameidentifier,
                        AuthProvider = AuthProviderType.GOOGLE,
                        User = user,
                        Status = Status.ACTIVE,
                    },
                    new UserAuthProvider() {
                        ProviderId = uuid,
                        AuthProvider = AuthProviderType.LOCAL,
                        User = user,
                        Status = Status.ACTIVE,
                    }
                }
            };
            user = await _repository.AddAsync(user);
            return user;
        }

        public async Task<User> GetUserIncludeFriendShip() {
            var userUUID = _httpContextAccessor.HttpContext?.User?.FindFirst(JwtType.UUID)?.Value;
            //var userUUID = "3ad5805b-2521-4e90-a2c4-50de632214f0";
            var user = await _repository.GetUserIncludeFriendShip(userUUID);
            return user;
        }

        public async Task<IEnumerable<User>> GetAllUser() {
            var userUUID = _httpContextAccessor.HttpContext?.User?.FindFirst(JwtType.UUID)?.Value;
            //var userUUID = "3ad5805b-2521-4e90-a2c4-50de632214f0";
            var listUser = await _repository.GetUsersWithoutFriendship(userUUID);
            return listUser;
        }

        public async Task<IEnumerable<User>> GetMutualFriends(string uuid1, string uuid2) {
            var listUserMutual = await _repository.GetMutualFriends(uuid1, uuid2);
            return listUserMutual;
        }

        public async Task<User> GetUserInforByUUID(string UUID) {
            var user = await _repository.GetUserInforByUUID(UUID);
            return user;
        }

        public async Task<List<User>> GetFriendRequestsForCurrentUser(string currentUserUuid) {
            return await _repository.GetFriendRequestsForCurrentUser(currentUserUuid);
        }

        public async Task<List<User>> GetFriendsByUuid(string userUUID) {
            return await _repository.GetFriendsByUuid(userUUID);
        }

        public async Task<User> GetProfile(long id) {
            var user = await _repository.GetProfile(id);
            return user;
        }
        public async Task<User> UpdateProfile(UserProfileDTO profile) {
            var user = await _repository.UpdateProfile(profile);
            return user;
        }
        public async Task<User> GetCurrentUserIncludeMessageAsync() {
            var userUUID = _httpContextAccessor.HttpContext?.User?.FindFirst(JwtType.UUID)?.Value;
            var user = await _repository.GetUserByUUIDIncludeNotificationAsync(userUUID);
            return user;
        }

        public async Task<IEnumerable<StatisticDTO>> GetStatisticUseInWeekAsync() {
            var users = await _repository.GetUserIncludeStatisticAsync();
            var statistics = new List<StatisticDTO>();
            foreach (var user in users) {
                statistics.Add(new StatisticDTO() {
                    UUID = user.UUID,
                    Email = user.Email,
                    Username = user.UserName,
                    NickName = user.Profile.NickName,
                    AccessTime = await _statisticService.GetAccessTimeByWeekAsync(user),
                    LastLogin = user.LastLogin,
                    AvatarUrl = _urlHelper.Action("GetImageInChat", "File", new { path = user.Profile.AvatarUrl }, _httpContextAccessor?.HttpContext?.Request.Scheme),
                });
            }
            return statistics;
        }

        public async Task UpdateLatLoginAsync(User user) {
            user.LastLogin = DateTime.Now;
            await _repository.UpdateAsync(user);
        }

        public async Task<IEnumerable<UserDtoForAdmin>> GetAllUserAsync() {
            var users = await _repository.GetAllUserAsync();
            var userDtoForAdmins = users.Select(u => new UserDtoForAdmin() {
                UUID = u.UUID,
                NickName = u.Profile.NickName,
                Email = u.Email,
                HasPassword = u.HasPassword,
                IsEmailConfirmed = u.IsEmailConfirmed,
                UserName = u.UserName,
                LastLogin = u.LastLogin,
                Role = u.Role.RoleName,
                Status = u.Status
            });
            return userDtoForAdmins;
        }

        public async Task<bool> BandUserAsync(string uuid) {
            var user = await GetUserInforByUUID(uuid);
            if (user == null) {
                return false;
            }
            if(user.Status == Status.BANNED) {
                user.Status = Status.ACTIVE;
            }else if(user.Status == Status.ACTIVE) {
                user.Status = Status.BANNED;
               
            }
            await UpdateAsync(user);
            return true;
        }
    }
}
