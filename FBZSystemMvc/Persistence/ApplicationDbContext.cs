using FBZSystemMvc.Persistence.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FBZSystemMvc.Persistence;

public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<SavedComic> SavedComics => Set<SavedComic>();
    public DbSet<SearchAnalyticsEvent> SearchAnalyticsEvents => Set<SearchAnalyticsEvent>();
    public DbSet<SearchResultHit> SearchResultHits => Set<SearchResultHit>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<SavedComic>()
            .HasIndex(x => new { x.UserId, x.ComicId })
            .IsUnique();

        builder.Entity<SearchResultHit>()
            .HasIndex(x => x.ComicId);
    }
}