using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FBZSystemMvc.Services.DatasetUpdates;

namespace FBZSystemMvc.Controllers.Staff;

[Authorize(Roles = "Staff")]
[Route("staff/dataset")]
public class DatasetUpdateController : Controller
{
    private readonly IDatasetUpdateService _updates;

    public DatasetUpdateController(IDatasetUpdateService updates)
    {
        _updates = updates;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        var status = _updates.GetStatus();
        return View(status);
    }

    [HttpPost("update-now")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateNow()
    {
        var (updated, message) = await _updates.CheckAndUpdateAsync(force: true);
        TempData["DatasetUpdateMessage"] = message;
        TempData["DatasetUpdateUpdated"] = updated ? "1" : "0";
        return Redirect("/staff/dataset");
    }
}