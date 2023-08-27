using Entities.Models;

namespace Repository.Contracts;

public interface IAuctionRepository
{
    void CreateRecord(Auction auction);
    void UpdateRecord(Auction auction);
    void DeleteRecord(Auction auction);
    Task<Auction> GetRecordById(int id);
    Task<IEnumerable<Auction>> GetAllAuctionsForUserId(int userId);
    Task<IEnumerable<Auction>> GetAllActiveAuctions();
}