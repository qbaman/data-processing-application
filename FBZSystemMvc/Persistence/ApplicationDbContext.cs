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
    public DbSet<FlaggedComic> FlaggedComics => Set<FlaggedComic>();

    public DbSet<SavedSearchList> SavedSearchLists => Set<SavedSearchList>();
    public DbSet<SavedSearchListItem> SavedSearchListItems => Set<SavedSearchListItem>();
    public DbSet<QueryStat> QueryStats => Set<QueryStat>();
    public DbSet<ComicResultStat> ComicResultStats => Set<ComicResultStat>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<SavedComic>()
            .HasIndex(x => new { x.UserId, x.ComicId })
            .IsUnique();

        builder.Entity<SearchResultHit>()
            .HasIndex(x => x.ComicId);

            builder.Entity<FlaggedComic>()
    .HasIndex(x => x.ComicId)
    .IsUnique();

builder.Entity<SavedSearchList>()
    .HasMany(x => x.Items)
    .WithOne(x => x.SavedSearchList)
    .HasForeignKey(x => x.SavedSearchListId)
    .OnDelete(DeleteBehavior.Cascade);

builder.Entity<SavedSearchList>()
    .HasIndex(x => new { x.UserId, x.Name })
    .IsUnique();

builder.Entity<SavedSearchListItem>()
    .HasIndex(x => new { x.SavedSearchListId, x.ComicId })
    .IsUnique();

builder.Entity<QueryStat>()
    .HasIndex(x => x.Signature)
    .IsUnique();

builder.Entity<ComicResultStat>()
    .HasIndex(x => x.ComicId)
    .IsUnique();

builder.Entity<SearchAnalyticsEvent>()
    .HasIndex(x => x.OccurredUtc);

builder.Entity<FlaggedComic>()
    .HasIndex(x => x.StaffUserId);
    }
}