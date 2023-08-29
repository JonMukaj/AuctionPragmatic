using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.DTO;

public class CreateBidDTO
{
    public decimal BidAmount { get; set; }
    public int AuctionId { get; set; }
}