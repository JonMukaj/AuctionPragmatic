using System.Security.Claims;
using Azure.Core;
using Entities.Exceptions;
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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateAuctionDTO request)
        {
            try
            {
                var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _serviceManger.AuctionService.CreateAuction(request, userId);
            }
            catch (DefaultException ex)
            {
                ModelState.AddModelError(ex.PropertyKey, ex.Message);
                return View(request);
            }
            return RedirectToAction("List");
        }

        
        public async Task<IActionResult> Details(int id)
        {
            var auction = await _serviceManger.AuctionService.GetAuctionById(id);
           
            if (TempData["ErrorMessage"] != null && TempData["PropertyKey"] != null)
            {
                //this is used only when we get an error from the BidController
                string errorMessage = TempData["ErrorMessage"].ToString();
                string propertyKey = TempData["PropertyKey"].ToString();
                ModelState.AddModelError(propertyKey, errorMessage);
            }
            return View(auction);
        }

        [HttpPost("delete/{auctionId}")]
        public async Task<IActionResult> Delete(int auctionId)
        {
            var result = await _serviceManger.AuctionService.DeleteAuction(auctionId);
            return RedirectToAction("List");
        }

    }
}
