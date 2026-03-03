import axios from "axios";
import {UserAchievementWithDetailsDto} from "../Dto/Achievements/UserAchievementWithDetailsDto";

export default class AchievementsApi {
    static init = () => {
        return axios.create({
            baseURL: `/webApi`,
            timeout: 10000,
            headers: {
                Accept: "application/json",
                "Content-Type": "application/json",
            },
            validateStatus: (status) => status < 500,
        });
    };

    static getByUser = async (userId: string): Promise<UserAchievementWithDetailsDto[]> => {
        const result = await AchievementsApi.init().get<UserAchievementWithDetailsDto[]>(`/users/${userId}/achievements`);
        if (result.status !== 200) {
            return [];
        }
        return result.data;
    };
}
