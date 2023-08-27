using System.Runtime.CompilerServices;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DTO;

namespace Service;

public class AuctionService:IAuctionService
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


    public async Task<bool> CreateAuction(CreateAuctionDTO request)
    {
        var auction = _mapper.Map<Auction>(request);
        auction.DateCreated = DateTime.Now;

        var user = await _repositoryManager.UserRepository.GetRecordById(request.UserId);
        if (user is null) throw new NotFoundException($"No user was found with id {request.UserId}");

        auction.UserId = request.UserId;

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

        return _mapper.Map<IEnumerable<GetAuctionDTO>>(list);
    }
}