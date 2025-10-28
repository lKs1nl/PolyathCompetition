using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PolyathlonCompetition.Web.Models;

namespace PolyathlonCompetition.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Competitor> Competitors { get; set; } = null!;
        public DbSet<Competition> Competitions { get; set; } = null!;
        public DbSet<CompetitionResult> CompetitionResults { get; set; } = null!;
        public DbSet<CompetitionRegistration> CompetitionRegistrations { get; set; } = null!; // ← НОВОЕ

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Настройка связей для регистраций
            builder.Entity<CompetitionRegistration>()
                .HasOne(cr => cr.Competitor)
                .WithMany(c => c.CompetitionRegistrations)
                .HasForeignKey(cr => cr.CompetitorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CompetitionRegistration>()
                .HasOne(cr => cr.Competition)
                .WithMany(c => c.CompetitionRegistrations)
                .HasForeignKey(cr => cr.CompetitionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Уникальный индекс - участник может быть зарегистрирован на соревнование только один раз
            builder.Entity<CompetitionRegistration>()
                .HasIndex(cr => new { cr.CompetitionId, cr.CompetitorId })
                .IsUnique();

            // Остальные настройки...
            builder.Entity<CompetitionResult>()
                .HasOne(cr => cr.Competitor)
                .WithMany()
                .HasForeignKey(cr => cr.CompetitorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CompetitionResult>()
                .HasOne(cr => cr.Competition)
                .WithMany()
                .HasForeignKey(cr => cr.CompetitionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}