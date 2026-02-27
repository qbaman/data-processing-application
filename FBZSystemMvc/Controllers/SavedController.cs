using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FBZ_System.Repositories;
using FBZSystemMvc.Models.Saved;
using FBZSystemMvc.Services;
using FBZSystemMvc.Services.Persistence;

namespace FBZSystemMvc.Controllers;

[Authorize]
public class SavedController : Controller
{
    private readonly UserManager<IdentityUser> _users;
    private readonly ISavedComicsService _saved;
    private readonly IComicRepository _repo;
    private readonly SearchListStore _sessionList;

    public SavedController(
        UserManager<IdentityUser> users,
        ISavedComicsService saved,
        IComicRepository repo,
        SearchListStore sessionList)
    {
        _users = users;
        _saved = saved;
        _repo = repo;
        _sessionList = sessionList;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = _users.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();

        var items = await _saved.GetForUserAsync(userId);
        return View(new SavedListViewModel { Items = items.ToList() });
    }

    [HttpPost]
    public async Task<IActionResult> Save(string id)
    {
        var userId = _users.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();

        var comic = _repo.GetAllComics().FirstOrDefault(c => c.Id == id);
        if (comic is null) return RedirectToAction("Index", "Dataset");

        await _saved.SaveAsync(userId, comic.Id);
        return RedirectToAction("Details", "Dataset", new { id });
    }

    [HttpPost]
    public async Task<IActionResult> Remove(string id)
    {
        var userId = _users.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();

        await _saved.RemoveAsync(userId, id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ImportFromSession()
    {
        var userId = _users.GetUserId(User);
        if (string.IsNullOrWhiteSpace(userId)) return Challenge();

        // We are not persisting session-import yet (method not implemented) — just clear session list for now.
        _sessionList.Clear(HttpContext);
        return RedirectToAction(nameof(Index));
    }
}