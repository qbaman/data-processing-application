using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FBZ_System.Repositories;
using FBZSystemMvc.Persistence;
using FBZSystemMvc.Persistence.Entities;
using FBZSystemMvc.Services;

namespace FBZSystemMvc.Controllers;

[Authorize]
public class SavedListsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly SearchListStore _sessionList;
    private readonly IComicRepository _repo;

    public SavedListsController(ApplicationDbContext db, SearchListStore sessionList, IComicRepository repo)
    {
        _db = db;
        _sessionList = sessionList;
        _repo = repo;
    }

    [HttpGet("/saved-lists")]
    public IActionResult Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var lists = _db.SavedSearchLists
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedUtc)
            .Select(x => new
            {
                x.Id,
                x.Name,
                x.CreatedUtc,
                Count = x.Items.Count
            })
            .ToList();

        return View(lists);
    }

    [HttpPost("/saved-lists/create-from-session")]
    [ValidateAntiForgeryToken]
    public IActionResult CreateFromSession(string name)
    {
        name ??= "";
        name = name.Trim();

        if (name.Length < 2)
            return BadRequest("Name must be at least 2 characters.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var ids = _sessionList.GetIds(HttpContext).Distinct().ToList();
        if (ids.Count == 0)
            return BadRequest("Session Search List is empty.");

        // prevent duplicate name per user (unique index also enforces)
        var exists = _db.SavedSearchLists.Any(x => x.UserId == userId && x.Name == name);
        if (exists)
            return BadRequest("You already have a list with that name.");

        var list = new SavedSearchList
        {
            UserId = userId,
            Name = name,
            CreatedUtc = DateTime.UtcNow,
            Items = ids.Select(id => new SavedSearchListItem { ComicId = id }).ToList()
        };

        _db.SavedSearchLists.Add(list);
        _db.SaveChanges();

        return Redirect("/saved-lists");
    }

    [HttpPost("/saved-lists/{id:int}/load-to-session")]
    [ValidateAntiForgeryToken]
    public IActionResult LoadToSession(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var list = _db.SavedSearchLists
            .Include(x => x.Items)
            .FirstOrDefault(x => x.Id == id && x.UserId == userId);

        if (list == null) return NotFound();

        // overwrite session list with this saved list
        _sessionList.Clear(HttpContext);
        foreach (var item in list.Items)
            _sessionList.Add(HttpContext, item.ComicId);

        return RedirectToAction("Index", "Dataset", new { Page = 1 });
    }

    [HttpPost("/saved-lists/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var list = _db.SavedSearchLists.FirstOrDefault(x => x.Id == id && x.UserId == userId);
        if (list == null) return NotFound();

        _db.SavedSearchLists.Remove(list);
        _db.SaveChanges();

        return Redirect("/saved-lists");
    }

    [HttpGet("/saved-lists/{id:int}")]
    public IActionResult Details(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var list = _db.SavedSearchLists
            .AsNoTracking()
            .Include(x => x.Items)
            .FirstOrDefault(x => x.Id == id && x.UserId == userId);

        if (list == null) return NotFound();

        var all = _repo.GetAllComics().ToList();
        var comics = list.Items
            .Select(i => all.FirstOrDefault(c => c.Id == i.ComicId))
            .Where(c => c != null)
            .ToList();

        ViewBag.ListName = list.Name;
        ViewBag.ListId = list.Id;

        return View(comics);
    }
}