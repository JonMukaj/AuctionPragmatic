using System.ComponentModel.DataAnnotations;

namespace Shared.DTO;

public class GetAuctionDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime? StartTime { get; set; }
    public int UserId { get; set; }
    public DateTime EndTime { get; set; }
    public decimal StartingBid { get; set; }
    public decimal CurrentBid { get; set; }
    public decimal? MaxBid { get; set; }
    public bool IsEnded { get; set; }
}