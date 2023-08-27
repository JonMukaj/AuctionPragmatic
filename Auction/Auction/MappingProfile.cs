using AutoMapper;
using Entities.Models;
using Shared.DTO;
using System.Drawing;
using System.Net.Sockets;

namespace Auction;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        //Auction
        CreateMap<Entities.Models.Auction, CreateAuctionDTO>().ReverseMap();
        CreateMap<Entities.Models.Auction, UpdateAuctionDTO>().ReverseMap();
        CreateMap<Entities.Models.Auction, GetAuctionDTO>().ReverseMap();
        CreateMap<Entities.Models.Auction, GetAuctionDetailDTO>().ReverseMap();

        //User
        CreateMap<ApplicationUser, GetUserDTO>().ReverseMap();
        CreateMap<ApplicationUser, UpdateUserDTO>().ReverseMap();
        CreateMap<ApplicationUser, RegisterUserDTO>().ReverseMap();

        //Bid
        CreateMap<Bid,CreateBidDTO>().ReverseMap();
    }
}