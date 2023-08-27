using Shared.DTO;

namespace Service.Contracts;

public interface IAuctionService
{
    Task<bool> CreateAuction(CreateAuctionDTO request, int loggedUser);
    Task<bool> DeleteAuction(int auctionId);
    Task<bool> UpdateAuction(int auctionId,UpdateAuctionDTO request);
    Task<IEnumerable<GetAuctionDTO>> GetAllAuctions();
}