using WebAPI.Constants;
using WebAPI.Models;
using WebAPI.Repositories;

namespace WebAPI.Services {
    public class StatisticService : BaseService<Statistic, StatisticRepository> {
        private UserRepository _userRepository;
        public StatisticService(StatisticRepository repository
            , UserRepository userRepository) : base(repository) {
            _userRepository = userRepository;
        }
        public async Task RecordTimeAsync(Statistic? statistic) {
            if (statistic == null) {
                throw new ArgumentNullException(nameof(statistic));
            }
            var s = await _repository.GetStatisticByUserAsync(statistic.User.UUID);
            var user = await _userRepository.GetUserByUUIDAsync(statistic.User.UUID);
            Statistic st = new Statistic() {
                User = user,
                LoginAt = statistic.LoginAt,
                LogoutAt = statistic.LogoutAt,
                Status = Status.ACTIVE
            };
            if (s != null) {
                var timeDifference = statistic.LoginAt - s.LogoutAt;
                if (timeDifference.TotalSeconds <= 5) {
                    s.LogoutAt = statistic.LogoutAt;
                    await _repository.UpdateAsync(s);
                } else {
                    await _repository.AddAsync(st);
                }
            } else {
                await _repository.AddAsync(st);
            }
        }

        public async Task<TimeSpan> GetAccessTimeByWeekAsync(User user) {
            TimeSpan totalAccessTime = TimeSpan.Zero;
            foreach (var item in user.Statistics)
            {
                var loginAt = item.LoginAt;
                var logoutAt = item.LogoutAt;
                totalAccessTime += logoutAt - loginAt;
            }
            return totalAccessTime;
        }
    }
}
