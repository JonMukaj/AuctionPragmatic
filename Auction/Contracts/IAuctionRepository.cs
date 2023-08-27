using Entities.Models;

namespace Contracts;

public interface IAuctionRepository
{
    void CreateRecord(Auction auction);
    void UpdateRecord(Auction auction);
    void DeleteRecord(Auction auction);
    Task<Auction> GetRecordById(int id);
    Task<IEnumerable<Auction>> GetAllAuctionsForUserId(int userId);
    Task<IEnumerable<Auction>> GetAllActiveAuctions();
    Task<IEnumerable<Auction>> GetEndedAuctions();
}