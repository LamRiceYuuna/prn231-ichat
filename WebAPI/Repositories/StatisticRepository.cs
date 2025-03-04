using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Repositories {
    public class StatisticRepository : BaseRepository<Statistic, TDbContext> {
        public StatisticRepository(TDbContext dbContext) : base(dbContext) {
        }

        public async Task<Statistic> GetStatisticByUserAsync(string userUUID) {
             var s=  await _dbContext.Statistics.OrderByDescending(s => s.UpdatedAt)
                                    .Include(u => u.User)
                                    .FirstOrDefaultAsync(u => u.User.UUID == userUUID);
            return s;

        }
    }
}
