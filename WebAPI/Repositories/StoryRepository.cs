using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Repositories {
    public class StoryRepository : BaseRepository<Story, TDbContext> {
        public StoryRepository(TDbContext dbContext) : base(dbContext) {
        }

        public async Task<List<Story>> GetStoryByUUID(string UUID)
        {
            var story = _dbContext.Stories.Include(u => u.User).Where(u => u.User.UUID == UUID).ToList();
            return story;
        }
        public async Task<Story> DeleteStory(int id)
        {
            var story = _dbContext.Stories.Find(id);
            return story;
        }
    }
}
