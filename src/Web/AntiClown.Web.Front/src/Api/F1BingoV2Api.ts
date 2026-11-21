import F1PredictionsApi from "./F1PredictionsApi";
import { F1BingoDetailsDto } from "../Dto/F1Bingo/F1BingoDetailsDto";

export default class F1BingoV2Api {
  static getOwnBoard = async (
    season: number,
    signal: AbortSignal,
  ): Promise<F1BingoDetailsDto> => {
    const api = F1PredictionsApi.initV2();
    const path = "bingo/boards/me";
    const options = { params: { season }, signal };
    const { data } = await api.get<F1BingoDetailsDto>(path, options);
    if (data.board || !data.canCreate) return data;
    return (await api.put<F1BingoDetailsDto>(path, undefined, options)).data;
  };
}
