using System.Runtime.CompilerServices;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DTO;

namespace Service;

public class AuctionService : IAuctionService
{
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly IRepositoryManager _repositoryManager;

    public AuctionService(ILoggerManager logger, IMapper mapper, IRepositoryManager repositoryManager)
    {
        _logger = logger;
        _mapper = mapper;
        _repositoryManager = repositoryManager;
    }


    public async Task<bool> CreateAuction(CreateAuctionDTO request, int loggedUser)
    {
        var auction = _mapper.Map<Auction>(request);
        auction.DateCreated = DateTime.Now;

        var user = await _repositoryManager.UserRepository.GetRecordById(loggedUser);
        if (user is null) throw new NotFoundException($"No user was found with id {loggedUser}");

        if (request.StartTime is null)
            auction.StartTime=DateTime.Now.ToLocalTime();

        await ValidateRequest(request);

        auction.UserId = loggedUser;
        auction.MaxBid = auction.StartingBid;

        _repositoryManager.AuctionRepository.CreateRecord(auction);
        await _repositoryManager.SaveAsync();
        return true;
    }

    public async Task<bool> DeleteAuction(int auctionId)
    {
       var auction=await _repositoryManager.AuctionRepository.GetRecordById(auctionId);
       if (auction is null) throw new NotFoundException($"No auction with id {auctionId}");

       var listOfBids = await _repositoryManager.BidRepository.GetBidsForAuctionId(auctionId);

       foreach (var bid in listOfBids ) 
       {
           _repositoryManager.BidRepository.DeleteRecord(bid);
       }

       _repositoryManager.AuctionRepository.DeleteRecord(auction);
       await _repositoryManager.SaveAsync();
       return true;
    }

    public Task<bool> UpdateAuction(int auctionId, UpdateAuctionDTO request)
    {
        throw new NotImplementedException();
    }

    public async Task<GetAuctionDetailDTO> GetAuctionById(int auctionId)
    {
        var auction = await _repositoryManager.AuctionRepository.GetRecordById(auctionId);
        if (auction is null) throw new NotFoundException($"No auction was found for id {auctionId}");

        var user = await _repositoryManager.UserRepository.GetRecordById(auction.UserId);
        if (user is null) throw new NotFoundException("No user was found for auction");

        var mapped = _mapper.Map<GetAuctionDetailDTO>(auction);

        var maxBid = await _repositoryManager.BidRepository.GetMaximumBid(auction.Id);
        if (maxBid is not null)
        {
            var maxBidder = await _repositoryManager.UserRepository.GetRecordById(maxBid.UserId);
            if (maxBidder is null) throw new NotFoundException("No maximum user bidder was found for auction");
            mapped.HighestBidAmount = maxBid.BidAmount;
            mapped.HighestBidder = string.Concat(maxBidder.FirstName, " ", maxBidder.LastName);
        }

        mapped.HighestBidAmount =auction.MaxBid.Value;
        mapped.CreatedBy = string.Concat(user.FirstName, " ", user.LastName);
        mapped.RemainingTime = await GetRemainingTime(auction.EndTime);

        return mapped;
    }

    public async Task<IEnumerable<GetAuctionDTO>> GetAllAuctions()
    {
        var list = await _repositoryManager.AuctionRepository.GetAllActiveAuctions();
        if (list is null)
            throw new NotFoundException("No list with auction were found!");

        var mapped = new List<GetAuctionDTO>();

        foreach (var auction in list)
        {
            var user = await _repositoryManager.UserRepository.GetRecordById(auction.UserId);
            var maxBid=await _repositoryManager.BidRepository.GetMaximumBid(auction.Id);

            var mappedAuction = _mapper.Map<GetAuctionDTO>(auction);

            mappedAuction.Username = string.Concat(user.FirstName + " " + user.LastName);
            mappedAuction.MaxBid = maxBid != null ? maxBid.BidAmount : auction.StartingBid;
            mappedAuction.RemainingTime = await GetRemainingTime(auction.EndTime);


            mapped.Add(mappedAuction);
        }

        return mapped;
    }


    #region private

    private async Task<string> GetRemainingTime(DateTime endTime)
    {
        var remaining = endTime.Subtract(DateTime.Now);

        if (remaining.Days >= 1)
        {
            return await Task.FromResult(string.Concat(remaining.Days.ToString("D"), " Days"));
        }
        else if (remaining.Hours >= 1 && remaining.Days < 1)
        {
            return await Task.FromResult(string.Concat(remaining.Hours.ToString("D"), " Hours"));
        }
        else
        {
            return await Task.FromResult(string.Concat(remaining.Minutes.ToString("D"), " Minutes"));
        }
        // return await  Task.FromResult(remaining.ToString("g"));
    }

    private async Task ValidateRequest(CreateAuctionDTO request)
    {
       
        if (request.Title.Length < 4) 
            throw new DefaultException("Title", "Title must be greater length than 3!");

        if (request.Description.Length < 11) 
            throw new DefaultException("Description", "Description must be greater length than 10!");

        if (request.StartTime < DateTime.Now)
            throw new DefaultException("StartTime", "Enter a valid date!");

        if (request.EndTime < DateTime.Now)
            throw new DefaultException("EndTime", "Enter a valid date!");

        if(request.StartingBid<1)
            throw new DefaultException("StartingBid", "Enter a positive amount!");

    }

    #endregion

}