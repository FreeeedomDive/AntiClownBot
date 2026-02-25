using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.ChampionshipPredictions;
using AntiClown.Entertainment.Api.Dto.F1Predictions.ChampionshipPredictions;

using AutoMapper;

namespace AntiClown.Entertainment.Api.Mappings;

public class F1ChampionshipPredictionsMapperProfile : Profile
{
    public F1ChampionshipPredictionsMapperProfile()
    {
        CreateMap<F1ChampionshipPrediction, F1ChampionshipPredictionDto>().ReverseMap();
    }
}
