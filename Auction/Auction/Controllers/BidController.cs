using Entities.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DTO;
using System.Security.Claims;

namespace Auction.Controllers
{
    [Authorize]
    public class BidController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public BidController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateBidDTO request)
        {
            try
            {
                var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _serviceManager.BidService.CreateBid(request, userId);
            }
            catch (DefaultException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                TempData["PropertyKey"] = ex.PropertyKey;
            }

            return RedirectToAction("Details","Auction",new {id=request.AuctionId});
        }
    }
}