using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class BidRepository:RepositoryBase<Bid>,IBidRepository
{
    public BidRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public void CreateRecord(Bid bid) => Create(bid);

    public void UpdateRecord(Bid bid) => Update(bid);

    public void DeleteRecord(Bid bid) => Delete(bid);

    public async Task<Bid> GetRecordById(int id) => await FindByCondition(e => e.Id.Equals(id)).FirstOrDefaultAsync();

    public async Task<IEnumerable<Bid>> GetBidsForUserId(int userId)=>await FindByCondition(e=>e.UserId.Equals(userId))
        .OrderByDescending(x=>x.BidAmount).ToListAsync();

    public async Task<IEnumerable<Bid>> GetBidsForAuctionId(int auctionId)=>await FindByCondition(e=>e.AuctionId.Equals(auctionId))
        .OrderByDescending(x=>x.BidAmount).ToListAsync();
}