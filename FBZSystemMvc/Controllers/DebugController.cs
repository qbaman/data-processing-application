using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBZSystemMvc.Controllers;

[Authorize]
public class DebugController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public DebugController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("/debug/whoami")]
    public async Task<IActionResult> WhoAmI()
    {
        var isAuth = User?.Identity?.IsAuthenticated ?? false;
        var name = User?.Identity?.Name ?? "(null)";

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Content($"IsAuthenticated: {isAuth}\nIdentity.Name: {name}\nUser: (null)");
        }

        var roles = await _userManager.GetRolesAsync(user);
        return Content($"IsAuthenticated: {isAuth}\nEmail: {user.Email}\nUserId: {user.Id}\nRoles: {(roles.Count == 0 ? "(none)" : string.Join(", ", roles))}");
    }
}