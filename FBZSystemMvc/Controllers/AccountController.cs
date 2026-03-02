using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FBZSystemMvc.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
    }

    // Works even if logout gets hit by GET (simple + reliable for your coursework demo)
    [HttpGet("/account/logout")]
    public async Task<IActionResult> LogoutGet(string? returnUrl = "/")
    {
        await _signInManager.SignOutAsync();
        return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
    }

    [HttpPost("/account/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogoutPost(string? returnUrl = "/")
    {
        await _signInManager.SignOutAsync();
        return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl);
    }
}