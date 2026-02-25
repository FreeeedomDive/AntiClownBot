import axios from "axios";
import { AddPredictionResultDto } from "../Dto/F1Predictions/AddPredictionResultDto";
import { F1ChampionshipPredictionDto } from "../Dto/F1Predictions/F1ChampionshipPredictionDto";
import { F1ChampionshipResultsDto } from "../Dto/F1Predictions/F1ChampionshipResultsDto";

export default class F1ChampionshipPredictionsApi {
  static init = () => {
    return axios.create({
      baseURL: `/webApi/f1ChampionshipPredictions/`,
      timeout: 10000,
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      validateStatus: function (status) {
        return status < 500;
      },
    });
  };

  static read = async (
    userId: string,
    season: number,
  ): Promise<F1ChampionshipPredictionDto> => {
    const result =
      await F1ChampionshipPredictionsApi.init().get<F1ChampionshipPredictionDto>(
        ``,
        { params: { userId, season } },
      );
    return result.data;
  };

  static createOrUpdate = async (
    prediction: F1ChampionshipPredictionDto,
  ): Promise<AddPredictionResultDto> => {
    const result = await F1ChampionshipPredictionsApi.init().post<AddPredictionResultDto>(
      ``,
      prediction,
    );
    return result.data;
  };

  static readResults = async (
    season: number,
  ): Promise<F1ChampionshipResultsDto> => {
    const result =
      await F1ChampionshipPredictionsApi.init().get<F1ChampionshipResultsDto>(
        `results`,
        { params: { season } },
      );
    return result.data;
  };

  static writeResults = async (
    season: number,
    dto: F1ChampionshipResultsDto,
  ): Promise<void> => {
    await F1ChampionshipPredictionsApi.init().post(`results`, dto, {
      params: { season },
    });
  };
}
