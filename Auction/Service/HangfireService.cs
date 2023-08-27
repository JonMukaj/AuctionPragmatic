using System.Runtime.InteropServices;
using Contracts;
using Entities.Models;
using Service.Contracts;

namespace Service;

public class HangfireService : IHangfireService
{
    private readonly IRepositoryManager _repositoryManager;
    public HangfireService(IRepositoryManager repositoryManager)
    {
        _repositoryManager = repositoryManager;
    }

    public async Task UpdateDataMarket()
    {
        await GetEndedAuctions();
    }


    #region private

    private async Task GetEndedAuctions()
    {
        var list = await _repositoryManager.AuctionRepository.GetEndedAuctions();

        foreach (var auction in list)
        {
            auction.IsEnded = true;
            
            _repositoryManager.AuctionRepository.UpdateRecord(auction);
            //handle the wallet part
        }

        await _repositoryManager.SaveAsync();
    }

    #endregion
}