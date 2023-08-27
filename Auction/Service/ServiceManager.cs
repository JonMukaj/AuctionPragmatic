using AutoMapper;
using Contracts;
using Cryptography;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Repository.Contracts;
using Service.Contracts;
using Shared.Utility;

namespace Service;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IUserService> _userService;
    private readonly Lazy<IAuctionService> _auctionService;
    private readonly Lazy<IBidService> _bidService;
    public ServiceManager(IRepositoryManager repositoryManager
    , IMapper mapper
    , ILoggerManager logger
    , UserManager<ApplicationUser> userManager
    , SignInManager<ApplicationUser> signInManager
    , ICryptoUtils cryptoUtils
    , IHubContext<SignalHub> hub
    , HttpClient httpClient

     )
    {
        _userService = new Lazy<IUserService>(() => new UserService(logger, mapper, repositoryManager, userManager, signInManager, cryptoUtils));
        _auctionService = new Lazy<IAuctionService>(() => new AuctionService(logger, mapper, repositoryManager));
        _bidService = new Lazy<IBidService>(() => new BidService(logger, mapper, repositoryManager));
    }
    public IUserService UserService => _userService.Value;
    public IAuctionService AuctionService => _auctionService.Value;
    public IBidService BidService => _bidService.Value;
}