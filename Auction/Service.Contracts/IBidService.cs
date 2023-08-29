using Shared.DTO;

namespace Service.Contracts;

public interface IBidService
{
    Task<bool> CreateBid(CreateBidDTO request, int userId);
}