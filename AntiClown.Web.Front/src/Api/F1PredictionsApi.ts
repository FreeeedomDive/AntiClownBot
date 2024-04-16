import axios from "axios";
import { F1PredictionDto } from "../Dto/F1Predictions/F1PredictionDto";
import { F1RaceDto } from "../Dto/F1Predictions/F1RaceDto";
import {AddPredictionResultDto} from "../Dto/F1Predictions/AddPredictionResultDto";
import {F1PredictionRaceResultDto} from "../Dto/F1Predictions/F1PredictionRaceResultDto";

export default class F1PredictionsApi {
  static init = () => {
    return axios.create({
      baseURL: `/webApi/f1predictions/`,
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

  static readAllActive = async (): Promise<F1RaceDto[]> => {
    const result = await F1PredictionsApi.init().get<F1RaceDto[]>(
      `active`
    );
    return result.data;
  };

  static addPrediction = async (
    raceId: string,
    prediction: F1PredictionDto
  ): Promise<AddPredictionResultDto> => {
    const result = await F1PredictionsApi.init().post(
      `/${raceId}/addPrediction`,
      prediction
    );
    return result.data;
  };

  static addResult = async (
    raceId: string,
    result: F1PredictionRaceResultDto
  ): Promise<void> => {
    await F1PredictionsApi.init().post(
      `/${raceId}/addResult`,
      result
    );
  };

  static finish = async (
    raceId: string
  ): Promise<void> => {
    await F1PredictionsApi.init().post(
      `/${raceId}/finish`
    );
  };
}
