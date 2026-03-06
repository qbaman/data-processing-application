using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FBZ_System.Repositories;
using FBZSystemMvc.Models.Staff;
using FBZSystemMvc.Persistence;
using FBZSystemMvc.Persistence.Entities;

namespace FBZSystemMvc.Controllers.Staff;

[Authorize(Roles = "Staff")]
[Route("staff/flags")]
public class FlagsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IComicRepository _repo;

    public FlagsController(ApplicationDbContext db, IComicRepository repo)
    {
        _db = db;
        _repo = repo;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var flags = _db.FlaggedComics
            .AsNoTracking()
            .OrderByDescending(f => f.CreatedUtc)
            .ToList();

        // Map IDs to titles (fallback to ID if missing)
        var comics = _repo.GetAllComics();
        var titleById = comics
            .GroupBy(c => c.Id)
            .ToDictionary(g => g.Key, g => g.First().MainTitle);

        var vm = new FlagsIndexViewModel
        {
            Flags = flags.Select(f => new FlagRow
            {
                ComicId = f.ComicId,
                Title = titleById.TryGetValue(f.ComicId, out var t) ? t : f.ComicId,
                Reason = f.Reason,
                CreatedUtc = f.CreatedUtc
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost("flag")]
    [ValidateAntiForgeryToken]
    public IActionResult Flag(string comicId, string? reason, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(comicId))
            return BadRequest();

        var existing = _db.FlaggedComics.FirstOrDefault(x => x.ComicId == comicId);
        var staffUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

        if (existing == null)
        {
            _db.FlaggedComics.Add(new FlaggedComic
            {
                ComicId = comicId,
                StaffUserId = staffUserId,
                Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim(),
                CreatedUtc = DateTime.UtcNow
            });
        }
        else
        {
            existing.StaffUserId = staffUserId;
            existing.Reason = string.IsNullOrWhiteSpace(reason) ? existing.Reason : reason.Trim();
            existing.CreatedUtc = DateTime.UtcNow;
        }

        _db.SaveChanges();

        return Redirect(string.IsNullOrWhiteSpace(returnUrl)
            ? Url.Action("Details", "Dataset", new { id = comicId })!
            : returnUrl);
    }

    [HttpPost("unflag")]
    [ValidateAntiForgeryToken]
    public IActionResult Unflag(string comicId, string? returnUrl = null)
    {
        if (string.IsNullOrWhiteSpace(comicId))
            return BadRequest();

        var existing = _db.FlaggedComics.FirstOrDefault(x => x.ComicId == comicId);
        if (existing != null)
        {
            _db.FlaggedComics.Remove(existing);
            _db.SaveChanges();
        }

        return Redirect(string.IsNullOrWhiteSpace(returnUrl)
            ? Url.Action("Details", "Dataset", new { id = comicId })!
            : returnUrl);
    }
}