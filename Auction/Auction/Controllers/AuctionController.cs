using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Service.Contracts;
using Shared.DTO;

namespace Auction.Controllers
{
    [Authorize]
    public class AuctionController : Controller
    {
        private readonly IServiceManager _serviceManger;

        public AuctionController(IServiceManager serviceManger)
        {
            _serviceManger = serviceManger;
        }


        public async Task<IActionResult> List()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var userBalance = HttpContext.User.FindFirstValue("WalletBalance");
                var test = HttpContext.User.Claims;
            }
            var result = await _serviceManger.AuctionService.GetAllAuctions();
            return View(result);
        }

     
        public async Task<IActionResult> Create()
        {
        //    var result = await _serviceManger.AuctionService.CreateAuction(request);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateAuctionDTO request)
        {
            var result = await _serviceManger.AuctionService.CreateAuction(request);
            return RedirectToAction("List");
        }

    }
}
