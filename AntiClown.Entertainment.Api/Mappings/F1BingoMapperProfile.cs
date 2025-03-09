using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Bingo;
using AntiClown.Entertainment.Api.Dto.F1Predictions.Bingo;
using AutoMapper;

namespace AntiClown.Entertainment.Api.Mappings;

public class F1BingoMapperProfile : Profile
{
    public F1BingoMapperProfile()
    {
        CreateMap<F1BingoCardProbabilityDto, F1BingoCardProbability>().ReverseMap();
        CreateMap<CreateF1BingoCardDto, CreateF1BingoCard>();
        CreateMap<UpdateF1BingoCardDto, UpdateF1BingoCard>();
        CreateMap<F1BingoCard, F1BingoCardDto>();
    }
}