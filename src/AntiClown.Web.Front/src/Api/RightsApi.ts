import axios from "axios";
import {RightsDto} from "../Dto/Rights/RightsDto";

export default class RightsApi {
  static init = () => {
    return axios.create({
      baseURL: `/webApi/rights/`, timeout: 10000, headers: {
        Accept: "application/json",
        "Content-Type": "application/json"
      },
      validateStatus: function (status) {
        return status < 500;
      }
    });
  }

  static getUserRights = async (userId: string): Promise<RightsDto[]> => {
    const result = await RightsApi.init().get<RightsDto[]>(`/${userId}`);
    return result.data;
  }
}