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
        CreateMap<SafetyCarsCount, F1SafetyCarsPredictionDto>().ReverseMap();
        CreateMap<DnfPrediction, F1DnfPredictionDto>().ReverseMap();
        CreateMap<F1Prediction, F1PredictionDto>().ReverseMap();
        CreateMap<F1PredictionRaceResult, F1PredictionRaceResultDto>().ReverseMap();
        CreateMap<F1PredictionResult, F1PredictionUserResultDto>();
        CreateMap<PredictionConditions, F1RacePredictionConditionsDto>().ReverseMap();
        CreateMap<F1Race, F1RaceDto>();
        CreateMap<F1RaceFilterDto, F1RaceFilter>();
        CreateMap<DriverStatistics, DriverStatisticsDto>();
        CreateMap<MostPickedDriversStats, MostPickedDriversStatsDto>();
        CreateMap<MostProfitableDriversStats, MostProfitableDriversStatsDto>();
        CreateMap<UserPointsStats, UserPointsStatsDto>();
        CreateMap<F1Team, F1TeamDto>().ReverseMap();
    }
}