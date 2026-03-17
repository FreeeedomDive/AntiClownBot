export type Rarity = "Common" | "Rare" | "Epic" | "Legendary" | "BlackMarket";

export type ItemType = "Positive" | "Negative";

export type ItemName =
  | "CatWife"
  | "CommunismBanner"
  | "DogWife"
  | "Internet"
  | "JadeRod"
  | "RiceBowl";

export interface BaseItemDto {
  id: string;
  ownerId: string;
  rarity: Rarity;
  price: number;
  isActive: boolean;
  itemType: ItemType;
  itemName: ItemName;
}

export interface CatWifeDto extends BaseItemDto {
  autoTributeChance: number;
}

export interface DogWifeDto extends BaseItemDto {
  lootBoxFindChance: number;
}

export interface InternetDto extends BaseItemDto {
  gigabytes: number;
  speed: number;
  ping: number;
}

export interface RiceBowlDto extends BaseItemDto {
  negativeRangeExtend: number;
  positiveRangeExtend: number;
}

export interface JadeRodDto extends BaseItemDto {
  length: number;
  thickness: number;
}

export interface CommunismBannerDto extends BaseItemDto {
  divideChance: number;
  stealChance: number;
}

export interface InventoryDto {
  catWives: CatWifeDto[];
  communismBanners: CommunismBannerDto[];
  dogWives: DogWifeDto[];
  internets: InternetDto[];
  jadeRods: JadeRodDto[];
  riceBowls: RiceBowlDto[];
  netWorth: number;
}
