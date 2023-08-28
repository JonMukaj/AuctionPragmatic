namespace Shared.DTO;

public class GetAuctionListWithUserDTO
{
    public IEnumerable<GetAuctionDTO>? AuctionDetails { get; set; }
    public GetUserDTO? UserDetails { get; set; }
}