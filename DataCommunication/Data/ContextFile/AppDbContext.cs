using Microsoft.EntityFrameworkCore;

namespace DataCommunication
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<IndividualUser> IndividualUsers { get; set; }
        public DbSet<CompanyUser> CompanyUsers { get; set; }
        public DbSet<DesignerUser> DesignerUsers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<AdminRole> AdminRoles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<EventInvitee> EventInvitees { get; set; }
        public DbSet<EventCongratulator> EventCongratulators { get; set; }
        public DbSet<EventMedia> EventMedia { get; set; }
        public DbSet<OfferCategory> OfferCategories { get; set; }
        public DbSet<PromotionalOffer> PromotionalOffers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // UserType seeding (optional)
            modelBuilder.Entity<UserType>().HasData(
                new UserType { Id = 1, Name = "Individual" },
                new UserType { Id = 2, Name = "Company" },
                new UserType { Id = 3, Name = "Designer" }
            );
            modelBuilder.Entity<AdminRole>().HasData(
                new AdminRole { Id = 1, Name = "SuperAdmin" },
                new AdminRole { Id = 2, Name = "Manager" }
            );

            modelBuilder.Entity<User>()
                .HasOne(u => u.IndividualProfile)
                .WithOne(i => i.User)
                .HasForeignKey<IndividualUser>(i => i.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.CompanyProfile)
                .WithOne(c => c.User)
                .HasForeignKey<CompanyUser>(c => c.UserId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.DesignerProfile)
                .WithOne(d => d.User)
                .HasForeignKey<DesignerUser>(d => d.UserId);
        }
    }
}
