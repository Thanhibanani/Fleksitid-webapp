using Fleksitid.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fleksitid.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Organization> Organizations => Set<Organization>();
        public DbSet<WorkContract> WorkContracts => Set<WorkContract>();
        public DbSet<TimeEntry> TimeEntries => Set<TimeEntry>();
        public DbSet<SickDay> SickDays => Set<SickDay>();
        public DbSet<ColleagueConsent> ColleagueConsents => Set<ColleagueConsent>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Organization>(entity =>
            {
                entity.HasIndex(o => o.OrgCode).IsUnique();

                entity.HasMany(o => o.Users)
                    .WithOne(u => u.Organization)
                    .HasForeignKey(u => u.OrganizationId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            builder.Entity<WorkContract>(entity =>
            {
                entity.Property(c => c.WeeklyHours).HasPrecision(5, 2);

                entity.HasOne(c => c.User)
                    .WithMany(u => u.WorkContracts)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<TimeEntry>(entity =>
            {
                entity.Property(t => t.ManualHours).HasPrecision(5, 2);
                entity.HasIndex(t => new { t.UserId, t.Date });

                entity.HasOne(t => t.User)
                    .WithMany(u => u.TimeEntries)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<SickDay>(entity =>
            {
                entity.HasOne(s => s.User)
                    .WithMany(u => u.SickDays)
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ColleagueConsent>(entity =>
            {
                entity.HasIndex(c => c.ShareToken).IsUnique();

                entity.HasOne(c => c.Owner)
                    .WithMany()
                    .HasForeignKey(c => c.OwnerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Viewer)
                    .WithMany()
                    .HasForeignKey(c => c.ViewerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
