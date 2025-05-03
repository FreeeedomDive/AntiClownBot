import axios from "axios";
import { F1RaceResultDto } from "../Dto/F1Predictions/F1RaceResultDto";

export default class F1FastApi {
  static init = () => {
    return axios.create({
      baseURL: `/webApi/f1predictions`,
      timeout: 120000,
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      validateStatus: function (status) {
        return status < 500;
      },
    });
  };

  static getRaceResult = async (raceId: string): Promise<F1RaceResultDto> => {
    const result = await F1FastApi.init().get<F1RaceResultDto>(`${raceId}/raceResult`);
    return result.data;
  };
}
