using AntiClown.Api.Core.Economies.Domain;
using AntiClown.Api.Core.Transactions.Domain;
using AntiClown.Api.Dto.Economies;
using AutoMapper;

namespace AntiClown.Api.Mappings;

public class EconomiesDtoMapperProfile : Profile
{
    public EconomiesDtoMapperProfile()
    {
        CreateMap<Transaction, TransactionDto>().ReverseMap();
        CreateMap<DateTimeRangeDto, DateTimeRange>();
        CreateMap<TransactionsFilterDto, TransactionsFilter>();
        CreateMap<Economy, EconomyDto>().ReverseMap();
        CreateMap<Tribute, TributeDto>();
        CreateMap<LohotronRewardType, LohotronRewardTypeDto>();
        CreateMap<LohotronReward, LohotronRewardDto>();
    }
}