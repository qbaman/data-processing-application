using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBZSystemMvc.Controllers;

[Authorize]
public class DebugController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public DebugController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
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

    [HttpGet("/debug/make-staff")]
    public async Task<IActionResult> MakeStaff()
    {
        const string roleName = "Staff";

        if (!await _roleManager.RoleExistsAsync(roleName))
            await _roleManager.CreateAsync(new IdentityRole(roleName));

        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        if (!await _userManager.IsInRoleAsync(user, roleName))
            await _userManager.AddToRoleAsync(user, roleName);

        return Content("OK - you are now Staff. Log out and log back in.");
    }
}