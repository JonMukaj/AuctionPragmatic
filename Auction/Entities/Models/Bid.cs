using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class Bid:BaseCreatedAndModified
{
    [Key] public int Id { get; set; }
    public decimal BidAmount { get; set; }
    public DateTime BidTime { get; set; }

    [ForeignKey("AuctionId")]
    public int? AuctionId { get; set; }
    public Auction? Auction { get; set; }


    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public ApplicationUser? User { get; set; }

}