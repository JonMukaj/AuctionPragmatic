using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTO;
using System.Security.Claims;
using Entities.Exceptions;

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
        try
        {
            var claims = await _serviceManger.UserService.Login(request.LoginUser);

            if (claims is not null)
            {
                await HttpContext.SignInAsync("MyCookieAuthenticationScheme", new ClaimsPrincipal(claims));
                return RedirectToAction("Index", "Home");

            }
        }
        catch (DefaultException ex)
        {
            //  ModelState.AddModelError("LoginUser.Username", ex.Message);
            ModelState.AddModelError(ex.PropertyKey, ex.Message);
            return View(request);
        }

        ViewData["ValidateMessage"] = "user is not logged in!";
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(LoginAndRegisterViewModel request)
    {
        try
        {
            var claims = await _serviceManger.UserService.SignUpUserAsync(request.RegisterUser);
            await HttpContext.SignInAsync("MyCookieAuthenticationScheme", new ClaimsPrincipal(claims));
        }
        catch (DefaultException ex)
        {
            //  ModelState.AddModelError("LoginUser.Username", ex.Message);
            ModelState.AddModelError(ex.PropertyKey, ex.Message);
            return View("Login",request);
        }
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("MyCookieAuthenticationScheme");
        Response.Cookies.Delete(".AspNetCore.Cookies");
        Response.Cookies.Delete(".AspNetCore.Identity.Application");
        Response.Cookies.Delete(".AspNetCore.Antiforgery.6AuIRqB3-IU");

        return RedirectToAction("Index", "Home");
    }
}