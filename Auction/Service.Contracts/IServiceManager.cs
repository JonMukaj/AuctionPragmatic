namespace Service.Contracts;

public interface IServiceManager
{
    IUserService UserService { get; }
    IAuctionService AuctionService { get; }
    IBidService BidService { get; }
}