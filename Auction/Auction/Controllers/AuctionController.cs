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
            var auctionList = await _serviceManger.AuctionService.GetAllAuctions();

            var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var loggedUser = await _serviceManger.UserService.GetUserById(userId);

            var result = new GetAuctionListWithUserDTO
            {
                AuctionDetails = auctionList,
                UserDetails = loggedUser
            };

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
            var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _serviceManger.AuctionService.CreateAuction(request,userId);
            return RedirectToAction("List");
        }

        [HttpGet("{auctionId}")]
        public async Task<IActionResult> Details(int auctionId)
        {
            var auction = await _serviceManger.AuctionService.GetAuctionById(auctionId);
            return View(auction);
        }

    }
}
