using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FBZSystemMvc.Data.Entities;

namespace FBZSystemMvc.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SavedComic> SavedComics => Set<SavedComic>();
    public DbSet<SearchAnalyticsEvent> SearchAnalytics => Set<SearchAnalyticsEvent>();
}