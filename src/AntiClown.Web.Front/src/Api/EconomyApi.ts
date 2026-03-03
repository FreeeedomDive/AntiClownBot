import axios from "axios";
import {EconomyDto} from "../Dto/Economy/EconomyDto";
import {TransactionDto} from "../Dto/Economy/TransactionDto";

export default class EconomyApi {
  static init = () => {
    return axios.create({
      baseURL: `/webApi/economy/`,
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

  static get = async (userId: string): Promise<EconomyDto> => {
    const result = await EconomyApi.init().get<EconomyDto>(
      userId
    );
    return result.data;
  }

  static getTransactions = async (userId: string, offset: number, limit: number): Promise<TransactionDto[]> => {
    const result = await EconomyApi.init().post<TransactionDto[]>(
      `${userId}/transactions`, {offset, limit}
    );
    return result.data;
  }
}