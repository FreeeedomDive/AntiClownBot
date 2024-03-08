import axios from "axios";
import { F1PredictionDto } from "../Dto/F1Predictions/F1PredictionDto";
import { F1RaceDto } from "../Dto/F1Predictions/F1RaceDto";

export default class F1PredictionsApi {
  static init = () => {
    return axios.create({
      baseURL: `/entertainmentApi/f1predictions/`,
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
    const result = await F1PredictionsApi.init().get<{ $values: F1RaceDto[] }>(
      `active`
    );
    return result.data.$values;
  };

  static addPrediction = async (
    raceId: string,
    prediction: F1PredictionDto
  ): Promise<boolean> => {
    const result = await F1PredictionsApi.init().post(
      `/${raceId}/addPrediction`,
      prediction
    );
    return result.status === 204;
  };
}
