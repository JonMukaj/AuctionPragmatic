using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DTO;

namespace Service;

public class BidService:IBidService
{
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryManager _repositoryManager;

    public BidService(ILoggerManager logger, IMapper mapper, IRepositoryManager repositoryManager)
    {
        _logger = logger;
        _mapper = mapper;
        _repositoryManager = repositoryManager;
    }

    public async Task<bool> CreateBid(CreateBidDTO request,int userId)
    {
        var bid = _mapper.Map<Bid>(request);

        var user = await _repositoryManager.UserRepository.GetRecordById(userId);
        if (user is null) throw new NotFoundException($"No user was found with id {userId}");

        var auction=await _repositoryManager.AuctionRepository.GetRecordById(request.AuctionId);
        if (auction is null) throw new NotFoundException($"No auction was found with id {request.AuctionId}");


        if (request.BidAmount > user.WalletBalance)
            throw new BadRequestException("User does not have enough balance!");

        if (request.BidAmount < auction.MaxBid)
            throw new BadRequestException("New bid should be higher that the maximum bid!");

        if (request.BidAmount > auction.MaxBid)
        {
            auction.MaxBid = request.BidAmount;
            _repositoryManager.AuctionRepository.UpdateRecord(auction);
        }

        var currentUserBid = await _repositoryManager.BidRepository.GetBidForUserIdAndAuctionId(userId, auction.Id);

        if (currentUserBid is not null)
        {
            var remainingBalance = request.BidAmount - currentUserBid.BidAmount;

            if (user.WalletBalance > remainingBalance) //user can bin
            {
                user.WalletBalance -= remainingBalance;
            }
        }
        else// user FIRST bid for the auction
        {
            user.WalletBalance -= request.BidAmount;
        }

        _repositoryManager.UserRepository.UpdateRecord(user);

        bid.BidTime = bid.DateCreated = DateTime.Now;
        bid.UserId=userId;

        //var maxBid = await _repositoryManager.BidRepository.GetMaximumBid(auction.Id);
        //if (maxBid is null) throw new NotFoundException($"No bid was found for auction with id{auction.Id}");


        _repositoryManager.BidRepository.CreateRecord(bid);
        await _repositoryManager.SaveAsync();
        return true;
    }

}