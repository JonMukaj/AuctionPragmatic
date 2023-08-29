namespace Shared.DTO;

public class UpdateAuctionDTO
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal StartingBid { get; set; }
    public decimal CurrentBid { get; set; }
    public decimal? MaxBid { get; set; }
}

public class CreateAuctionDTO : UpdateAuctionDTO
{
    public int UserId { get; set; }
}