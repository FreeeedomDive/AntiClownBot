using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;
using AutoMapper;

namespace AntiClown.Entertainment.Api.Mappings;

public class F1PredictionsMapperProfile : Profile
{
    public F1PredictionsMapperProfile()
    {
        CreateMap<F1Driver, F1DriverDto>().ReverseMap();
        CreateMap<F1Prediction, F1PredictionDto>().ReverseMap();
        CreateMap<F1PredictionResult, F1PredictionResultDto>();
        CreateMap<F1Race, F1RaceDto>();
        CreateMap<MostPickedDriversByUsersStats, MostPickedDriversByUsersStatsDto>();
        CreateMap<MostProfitableDriversStats, MostProfitableDriversStatsDto>();
    }
}