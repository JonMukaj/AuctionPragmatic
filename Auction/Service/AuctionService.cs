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

        auction.UserId = loggedUser;
        auction.MaxBid = auction.StartingBid;

        _repositoryManager.AuctionRepository.CreateRecord(auction);
        await _repositoryManager.SaveAsync();
        return true;
    }

    public Task<bool> DeleteAuction(int auctionId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateAuction(int auctionId, UpdateAuctionDTO request)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<GetAuctionDTO>> GetAllAuctions()
    {
        var list = await _repositoryManager.AuctionRepository.GetAllActiveAuctions();
        if (list is null)
            throw new NotFoundException("No list with auction were found!");

        var mapped = new List<GetAuctionDTO>();

        foreach (var i in list)
        {
            var user = await _repositoryManager.UserRepository.GetRecordById(i.UserId);

            var auction = _mapper.Map<GetAuctionDTO>(i);
            auction.Username = string.Concat(user.FirstName + " " + user.LastName);

           auction.RemainingTime = await GetRemainingTime(i.EndTime);
         

            mapped.Add(auction);
        }

        return mapped;
    }


    #region private

    private async Task<string> GetRemainingTime(DateTime endTime)
    {
        var remaining = endTime.Subtract(DateTime.Now);

        if (remaining.Days >= 1)
        {
            return await Task.FromResult( string.Concat(remaining.Days.ToString("D")," Days"));
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

    #endregion

}