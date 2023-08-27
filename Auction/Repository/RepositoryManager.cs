using Contracts;
using Repository.Contracts;

namespace Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;
    private readonly Lazy<IUserRepository> _userRepository;
    private readonly Lazy<IAuctionRepository> _auctionRepository;
    private readonly Lazy<IBidRepository> _bidRepository;
    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
        _userRepository = new Lazy<IUserRepository>(() => new UserRepository(repositoryContext));
        _auctionRepository = new Lazy<IAuctionRepository>(() => new AuctionRepository(repositoryContext));
        _bidRepository = new Lazy<IBidRepository>(() => new BidRepository(repositoryContext));
    }


    public IUserRepository UserRepository => _userRepository.Value;
    public IAuctionRepository AuctionRepository => _auctionRepository.Value;
    public IBidRepository BidRepository => _bidRepository.Value;

    public async Task SaveAsync()
    {
        _repositoryContext.ChangeTracker.AutoDetectChangesEnabled = false;
        await _repositoryContext.SaveChangesAsync();
    }
}