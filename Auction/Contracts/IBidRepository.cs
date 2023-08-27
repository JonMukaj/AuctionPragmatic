using Entities.Models;

namespace Contracts;

public interface IBidRepository
{
    void CreateRecord(Bid bid);
    void UpdateRecord(Bid bid);
    void DeleteRecord(Bid bid);
    Task<Bid> GetRecordById(int id);
    Task<IEnumerable<Bid>> GetBidsForUserId(int userId);
    Task<IEnumerable<Bid>> GetBidsForAuctionId(int auctionId);
}