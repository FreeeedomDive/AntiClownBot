import axios from "axios";
import { InventoryDto } from "../Dto/Inventory/InventoryDto";

export default class InventoryApi {
  static init = () => {
    return axios.create({
      baseURL: `/webApi/inventories/`,
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

  static get = async (userId: string): Promise<InventoryDto> => {
    const result = await InventoryApi.init().get<InventoryDto>(userId);
    return result.data;
  };

  static changeActiveStatus = async (
    userId: string,
    itemId: string,
    isActive: boolean
  ): Promise<void> => {
    await InventoryApi.init().post(
      `${userId}/items/${itemId}/active/${isActive}`
    );
  };

  static sell = async (userId: string, itemId: string): Promise<void> => {
    await InventoryApi.init().post(`${userId}/items/${itemId}/sell`);
  };
}
