import {SettingsCategory} from "./SettingsCategory";

export interface SettingDto {
    category: SettingsCategory;
    name: string;
    value: string;
}