using AutoMapper;
using Coinbot.Bittrex.Models;
using Coinbot.Domain.Contracts.Models.StockApiService;

namespace Coinbot.Bittrex.Mappings
{
    public class BittrexProfile : Profile
    {
        public BittrexProfile()
        {
            CreateMap<TickDTOResult, Tick>()
                .ForMember(dest => dest.Ask, opt => opt.MapFrom(src => src.Ask))
                .ForMember(dest => dest.Bid, opt => opt.MapFrom(src => src.Bid))
                .ForMember(dest => dest.Last, opt => opt.MapFrom(src => src.Last));

            CreateMap<TransactionDTO, Transaction>()
                .ForMember(dest => dest.OrderRefId, opt => opt.MapFrom(src => src.OrderUuid))
                .ForMember(dest => dest.IsOpen, opt => opt.MapFrom(src => src.IsOpen))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<TransactionMadeDTO, Transaction>()
                .ForMember(dest => dest.OrderRefId, opt => opt.MapFrom(src => src.uuid));


        }
    }
}