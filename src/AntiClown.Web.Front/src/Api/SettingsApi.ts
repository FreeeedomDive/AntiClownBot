import axios from "axios";
import {SettingsCategory} from "../Dto/Settings/SettingsCategory";
import {SettingDto} from "../Dto/Settings/SettingDto";

export default class SettingsApi {
    static init = () => {
        return axios.create({
            baseURL: `/webApi/settings/`, timeout: 10000, headers: {
                Accept: "application/json",
                "Content-Type": "application/json"
            },
            validateStatus: function (status) {
                return status < 500;
            }
        });
    }

    static find = async (category: SettingsCategory): Promise<SettingDto[]> => {
        const result = await SettingsApi.init().get(`/categories/${category}`);
        return result.data;
    }

    static save = async (setting: SettingDto): Promise<void> => {
        await SettingsApi.init().post(``, setting);
    }
}