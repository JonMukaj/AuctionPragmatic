using Repository.Contracts;

namespace Contracts;

public interface IRepositoryManager
{
    IUserRepository UserRepository { get; }
    IAuctionRepository AuctionRepository { get; }
    IBidRepository BidRepository { get; }
    Task SaveAsync();
}