using AntiClown.Entertainment.Api.Core.F1Predictions.Domain;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Predictions;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Results;
using AntiClown.Entertainment.Api.Core.F1Predictions.Domain.Stats;
using AntiClown.Entertainment.Api.Dto.F1Predictions;
using AntiClown.Entertainment.Api.Dto.F1Predictions.Statistics;
using AutoMapper;

namespace AntiClown.Entertainment.Api.Mappings;

public class F1PredictionsMapperProfile : Profile
{
    public F1PredictionsMapperProfile()
    {
        CreateMap<F1Driver, F1DriverDto>().ReverseMap();
        CreateMap<F1SafetyCars, F1SafetyCarsPredictionDto>().ReverseMap();
        CreateMap<F1DnfPrediction, F1DnfPredictionDto>().ReverseMap();
        CreateMap<F1Prediction, F1PredictionDto>().ReverseMap();
        CreateMap<F1PredictionResult, F1PredictionUserResultDto>();
        CreateMap<F1Race, F1RaceDto>();
        CreateMap<DriverStatistics, DriverStatisticsDto>();
        CreateMap<MostPickedDriversStats, MostPickedDriversStatsDto>();
        CreateMap<MostProfitableDriversStats, MostProfitableDriversStatsDto>();
        CreateMap<UserPointsStats, UserPointsStatsDto>();
    }
}