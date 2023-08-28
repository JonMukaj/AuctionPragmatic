using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Auction.Controllers;

public class ErrorController : Controller
{
    [Route("Error")]
    [AllowAnonymous]
    public IActionResult Error()
    {
        var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        ViewBag.ExceptionPath = exceptionDetails.Path;
        ViewBag.ExceptionMessage = exceptionDetails.Error.Message;
        ViewBag.Stacktrace = exceptionDetails.Error.StackTrace;

        return View("Error");
    }
}