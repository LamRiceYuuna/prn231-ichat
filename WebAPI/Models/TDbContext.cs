using Microsoft.EntityFrameworkCore;
using WebAPI.Constants;

namespace WebAPI.Models {
    public class TDbContext : DbContext {
        public virtual DbSet<BlockedUser> BlockedUsers { get; set; }
        public virtual DbSet<Chat> Chats { get; set; }
        public virtual DbSet<ChatMember> ChatMembers { get; set; }
        public virtual DbSet<File> Files { get; set; }
        public virtual DbSet<Friendship> Friendships { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<MessageFlag> MessageFlags { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Statistic> Statistics { get; set; }
        public virtual DbSet<Story> Stories { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAuthProvider> UserAuthProviders { get; set; }
        public virtual DbSet<UserNotification> UserNotifications { get; set; }


        public TDbContext() {
        }
        public TDbContext(DbContextOptions<TDbContext> options)
            : base(options) {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Profile>()
                .HasIndex(p => p.UserId)
                .IsUnique();

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<BlockedUser>()
                 .HasKey(bu => new { bu.UserId, bu.BlockedId });

            modelBuilder.Entity<BlockedUser>()
                .HasOne(bu => bu.User)
                .WithMany(u => u.BlockedUsers)
                .HasForeignKey(bu => bu.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BlockedUser>()
                .HasOne(bu => bu.Blocked)
                .WithMany(u => u.UsersBlocked)
                .HasForeignKey(bu => bu.BlockedId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ChatMember>()
                .HasOne(cm => cm.User)
                .WithMany(u => u.ChatMembers)
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Friendship>()
                .HasOne(fs => fs.User)
                .WithMany(u => u.Friendships)
                .HasForeignKey(fs => fs.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                .HasMany(m => m.MessageFlags)
                .WithOne(mf => mf.Message)
                .HasForeignKey(mf => mf.MessageId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Report>()
               .HasOne(r => r.Reporter)
               .WithMany(u => u.ReportedUsers)
               .HasForeignKey(r => r.ReporterId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.ReportedUser)
                .WithMany(u => u.Reporters)
                .HasForeignKey(r => r.ReportedUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Friendship>()
                .HasKey(fs => new { fs.UserId, fs.FriendUserId });

            modelBuilder.Entity<UserNotification>()
               .HasKey(un => new { un.UserId, un.NotificationId });

            List<Role> roles = new List<Role>() {
                new Role {RoleId =1,  RoleName = RoleType.USER, Status = Status.ACTIVE },
                new Role {RoleId =2, RoleName = RoleType.ADMIN, Status = Status.ACTIVE},
                new Role {RoleId =3, RoleName = RoleType.MEMBER, Status = Status.ACTIVE}
            };
            modelBuilder.Entity<Role>().HasData(roles);

            modelBuilder.Entity<User>().HasData(
                new User {UserId = 1, UUID = Guid.NewGuid().ToString(), UserName = "ichat", Email = "ichat@gmail.com", 
                    IsEmailConfirmed = true, Password = "12345678", HasPassword = true, LastLogin = DateTime.UtcNow,
                    RoleId = roles[0].RoleId, Status = Status.ACTIVE}
                );
            modelBuilder.Entity<Profile>().HasData(
                new Profile {ProfileId =1, UserId = 1, NickName = "IChat", AvatarUrl = "https://images.spiderum.com/sp-images/e9fef2d083cf11ea8f996dbfbe6e50b1.jpg", Status = Status.ACTIVE }
                );
        }
        public override int SaveChanges() {
            SetAutoUpdateTime();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) {
            SetAutoUpdateTime();
            return base.SaveChangesAsync(cancellationToken);
        }
        private void SetAutoUpdateTime() {
            var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            foreach (var entityEntry in entries) {
                ((BaseEntity)entityEntry.Entity).UpdatedAt = DateTime.Now;

                if (entityEntry.State == EntityState.Added) {
                    ((BaseEntity)entityEntry.Entity).CreatedAt = DateTime.Now;
                }
            }
        }
    }
}
