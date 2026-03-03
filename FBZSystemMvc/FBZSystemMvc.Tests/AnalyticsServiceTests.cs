using System.Security.Claims;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using FBZSystemMvc.Persistence;
using FBZSystemMvc.Services.Persistence;
using FBZ_System.Domain;

namespace FBZSystemMvc.Tests;

public class AnalyticsServiceTests
{
    private static ClaimsPrincipal FakeUser(string userId = "test-user")
    {
        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, "tester@example.com")
        }, "TestAuth");

        return new ClaimsPrincipal(identity);
    }

    private static ApplicationDbContext CreateDb(out SqliteConnection conn)
    {
        conn = new SqliteConnection("Data Source=:memory:");
        conn.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(conn)
            .Options;

        var db = new ApplicationDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    [Fact]
    public void RecordSearch_Page1_Increments_Query_And_Result_Counters()
    {
        using var db = CreateDb(out var conn);
        using (conn)
        {
            var svc = new AnalyticsService(db);

            var q = new SearchQuery { TitleContains = "bat", Page = 1 };

            var comics = new List<Comic>
            {
                new Comic { Id = "C1", MainTitle = "Batman" },
                new Comic { Id = "C2", MainTitle = "Batgirl" }
            };

            svc.RecordSearch(q, comics, FakeUser());

            Assert.Equal(1, db.QueryStats.Count());
            Assert.Equal(1, db.QueryStats.Single().Count);

            Assert.Equal(2, db.ComicResultStats.Count());
            Assert.Equal(1, db.ComicResultStats.Single(x => x.ComicId == "C1").Count);
            Assert.Equal(1, db.ComicResultStats.Single(x => x.ComicId == "C2").Count);

            Assert.Equal(1, db.SearchAnalyticsEvents.Count());
        }
    }

    [Fact]
    public void RecordSearch_Page2_DoesNotIncrement()
    {
        using var db = CreateDb(out var conn);
        using (conn)
        {
            var svc = new AnalyticsService(db);

            var q = new SearchQuery { TitleContains = "bat", Page = 2 };

            var comics = new List<Comic>
            {
                new Comic { Id = "C1", MainTitle = "Batman" }
            };

            svc.RecordSearch(q, comics, FakeUser());

            Assert.Equal(0, db.QueryStats.Count());
            Assert.Equal(0, db.ComicResultStats.Count());
            Assert.Equal(0, db.SearchAnalyticsEvents.Count());
        }
    }
}