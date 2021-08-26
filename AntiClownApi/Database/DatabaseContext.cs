using AntiClownBotApi.Database.DBModels;
using AntiClownBotApi.Database.DBModels.DbItems;
using Microsoft.EntityFrameworkCore;

namespace AntiClownBotApi.Database
{
    public sealed class DatabaseContext : DbContext
    {
        public DbSet<DbUser> Users { get; set; }
        public DbSet<DbItem> Items { get; set; }
        public DbSet<DbItemStats> ItemStats { get; set; }
        public DbSet<DbEmote> Emotes { get; set; }
        public DbSet<DbUserEmotes> UserEmotes { get; set; }
        public DbSet<DbUserEconomy> UserEconomies { get; set; }
        public DbSet<DbUserStats> UserStats { get; set; }
        public DbSet<DbShopItem> ShopItems { get; set; }
        public DbSet<DbUserShop> UserShops { get; set; }
        public DbSet<DbTransaction> Transactions { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DatabaseContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DbItem>()
                .HasOne(i => i.User)
                .WithMany(u => u.Items)
                .HasForeignKey(i => i.UserId);

            modelBuilder.Entity<DbItemStats>()
                .HasOne(s => s.Item)
                .WithOne(i => i.ItemStats)
                .HasForeignKey<DbItemStats>(s => s.ItemId);

            modelBuilder.Entity<DbUserShop>()
                .HasOne(s => s.User)
                .WithOne(u => u.Shop)
                .HasForeignKey<DbUserShop>(s => s.UserId);

            modelBuilder.Entity<DbShopItem>()
                .HasOne(i => i.Shop)
                .WithMany(s => s.Items)
                .HasForeignKey(i => i.ShopId);

            modelBuilder.Entity<DbUserEconomy>()
                .HasOne(e => e.User)
                .WithOne(u => u.Economy)
                .HasForeignKey<DbUserEconomy>(e => e.UserId);

            modelBuilder.Entity<DbTransaction>()
                .HasOne(e => e.UserEconomy)
                .WithMany(u => u.Transactions)
                .HasForeignKey(e => e.UserEconomyId);

            modelBuilder.Entity<DbUserStats>()
                .HasOne(e => e.User)
                .WithOne(u => u.Stats)
                .HasForeignKey<DbUserStats>(s => s.UserId);

            modelBuilder.Entity<DbUserEmotes>()
                .HasOne(e => e.UserStats)
                .WithMany(u => u.UsedEmotes)
                .HasForeignKey(ue => ue.StatsId);

            modelBuilder.Entity<DbEmote>()
                .HasMany(e => e.EmoteStats)
                .WithOne(e => e.Emote)
                .HasForeignKey(e => e.StatsId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(Utility.GetPosgreSqlConfigureStringFromFile());
        }
    }
}