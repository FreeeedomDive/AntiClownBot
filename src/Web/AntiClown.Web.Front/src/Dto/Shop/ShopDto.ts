import { ItemName, Rarity } from "../Inventory/InventoryDto";

export interface ShopItemDto {
  id: string;
  shopId: string;
  name: ItemName;
  rarity: Rarity;
  price: number;
  isRevealed: boolean;
  isOwned: boolean;
}

export interface CurrentShopInfoDto {
  id: string;
  reRollPrice: number;
  freeReveals: number;
  items: ShopItemDto[];
}

export interface ShopStatsDto {
  id: string;
  totalReRolls: number;
  itemsBought: number;
  totalReveals: number;
  scamCoinsLostOnReveals: number;
  scamCoinsLostOnReRolls: number;
  scamCoinsLostOnPurchases: number;
}
