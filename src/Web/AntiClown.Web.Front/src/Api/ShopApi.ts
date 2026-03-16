import axios from "axios";
import { CurrentShopInfoDto, ShopItemDto, ShopStatsDto } from "../Dto/Shop/ShopDto";

export default class ShopApi {
  static init = () => {
    return axios.create({
      baseURL: `/webApi/shops/`,
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

  static get = async (userId: string): Promise<CurrentShopInfoDto> => {
    const result = await ShopApi.init().get<CurrentShopInfoDto>(userId);
    return result.data;
  };

  static reveal = async (userId: string, itemId: string): Promise<ShopItemDto> => {
    const result = await ShopApi.init().post<ShopItemDto>(
      `${userId}/items/${itemId}/reveal`
    );
    return result.data;
  };

  static buy = async (userId: string, itemId: string): Promise<void> => {
    await ShopApi.init().post(`${userId}/items/${itemId}/buy`);
  };

  static reroll = async (userId: string): Promise<void> => {
    await ShopApi.init().post(`${userId}/reroll`);
  };

  static getStats = async (userId: string): Promise<ShopStatsDto> => {
    const result = await ShopApi.init().get<ShopStatsDto>(`${userId}/stats`);
    return result.data;
  };
}
