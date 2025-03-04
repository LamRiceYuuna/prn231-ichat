using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using WebAPI.Models;
using WebAPI.Constants;
using System.ComponentModel.Design;

namespace WebAPI.Repositories
{
    public class FileRepository : BaseRepository<Models.File, TDbContext>
    {
        public FileRepository(TDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Models.File>> GetFileByUUID(String UUID)
        {
            string status = Status.ACTIVE;
            var file = await _dbContext.Files.Include(f => f.Message)
                                               .ThenInclude(m => m.ChatMember)
                                               .ThenInclude(cm => cm.Chat)
                                               .Where(f => f.Message.ChatMember.Chat.UUID == UUID && f.Status == status)
                                               .ToListAsync();
            return file;
        }

        public async Task<Models.File> GetFileOfUserAsync(string uuid, string path, string type) {
            var file = await _dbContext.Files.Include(f => f.Message)
                                            .ThenInclude(m => m.ChatMember)
                                                .ThenInclude( c => c.User)
                                        .FirstOrDefaultAsync( 
                                                f=> f.Message.ChatMember.User.UUID == uuid 
                                                && f.Type == type);
            return file;
        }
        public virtual async Task<List<Models.File>> AddAllAsync(List<Models.File> files) {
            if (files.IsNullOrEmpty()) {
                throw new ArgumentNullException(nameof(files));
            }

            await _dbContext.Set<Models.File>().AddRangeAsync(files);
            await _dbContext.SaveChangesAsync();
            return files;
        }

    }
}