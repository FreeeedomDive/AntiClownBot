import axios from "axios";
import { F1BingoCardDto } from "../Dto/F1Bingo/F1BingoCardDto";
import {UpdateF1BingoCardDto} from "../Dto/F1Bingo/UpdateF1BingoCardDto";

export default class F1BingoApi {
  static init = () => {
    return axios.create({
      baseURL: `/webApi/f1Bingo/`,
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

  static getCards = async (season: number): Promise<F1BingoCardDto[]> => {
    const result = await F1BingoApi.init().get<F1BingoCardDto[]>(`cards`, {
      params: { season },
    });
    return result.data;
  };

  static updateCard = async (cardId: string, dto: UpdateF1BingoCardDto): Promise<void> => {
    await F1BingoApi.init().patch<string[]>(`cards/${cardId}`, dto);
  }

  static getBoard = async (userId: string, season: number): Promise<string[]> => {
    const result = await F1BingoApi.init().get<string[]>(`boards/${userId}`, {
      params: { season },
    });
    return result.data;
  }
}
