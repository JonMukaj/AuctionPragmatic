using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTO;
using System.Security.Claims;

namespace Auction.Controllers;

public class AuthenticationController : Controller
{
    private readonly IServiceManager _serviceManger;

    public AuthenticationController(IServiceManager serviceManger)
    {
        _serviceManger = serviceManger;
    }
    public IActionResult Login()
    {
        ClaimsPrincipal claimUser = HttpContext.User;
        if (claimUser.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Login(LoginAndRegisterViewModel request)
    {
        var claims = await _serviceManger.UserService.Login(request.LoginUser);

        if (claims is not null)
        {
            var properties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                IsPersistent = true,
                RedirectUri = "/Authentication/Login"
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claims),properties);
            return RedirectToAction("Index", "Home");

        }

        ViewData["ValidateMessage"] = "user is not logged in!";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(LoginAndRegisterViewModel request)
    {
        var claims = await _serviceManger.UserService.SignUpUserAsync(request.RegisterUser);

        var properties = new AuthenticationProperties()
        {
            AllowRefresh = true,
            IsPersistent = true,
            RedirectUri = "/Authentication/Login"
        };
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claims), properties);
    //    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claims));
        return RedirectToAction("Login", "Authentication");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete(".AspNetCore.Cookies");
        Response.Cookies.Delete(".AspNetCore.Identity.Application");
        Response.Cookies.Delete(".AspNetCore.Antiforgery.6AuIRqB3-IU");

        return RedirectToAction("Index", "Home");
    }
}