using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Contracts;

namespace Repository;

public class AuctionRepository : RepositoryBase<Auction>, IAuctionRepository
{
    public AuctionRepository(RepositoryContext repositoryContext) : base(repositoryContext)
    {
    }

    public void CreateRecord(Auction auction) => Create(auction);

    public void UpdateRecord(Auction auction) => Update(auction);

    public void DeleteRecord(Auction auction) => Delete(auction);

    public async Task<Auction> GetRecordById(int id) =>
        await FindByCondition(e => e.Id.Equals(id)).FirstOrDefaultAsync();

    public async Task<IEnumerable<Auction>> GetAllAuctionsForUserId(int userId) =>
        await FindByCondition(e => e.UserId.Equals(userId) && e.IsEnded.Equals(false)).ToListAsync();

    public async Task<IEnumerable<Auction>> GetAllActiveAuctions() =>
        await FindByCondition(e => e.IsEnded.Equals(false) && e.EndTime>=DateTime.Now && e.StartTime<=DateTime.Now).OrderBy(x => x.EndTime).ToListAsync();

    public async Task<IEnumerable<Auction>> GetEndedAuctions() =>
        await FindByCondition(e => e.EndTime <= DateTime.Now && e.IsEnded.Equals(false)).ToListAsync();
}