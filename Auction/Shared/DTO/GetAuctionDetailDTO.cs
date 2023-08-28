namespace Shared.DTO;

public class GetAuctionDetailDTO
{

    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string CreatedBy { get; set; }
    public string HighestBidder { get; set; }
    public decimal HighestBidAmount { get; set; }
    public bool IsEnded { get; set; }
    public string RemainingTime { get; set; }
    public decimal? BidAmount { get; set; }
}