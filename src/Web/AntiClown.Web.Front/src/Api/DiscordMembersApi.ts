import axios from "axios";
import {DiscordMemberDto} from "../Dto/Users/DiscordMemberDto";

export default class DiscordMembersApi {
  static init = () => {
    return axios.create({
      baseURL: `/webApi/discordMembers/`, timeout: 10000, headers: {
        Accept: "application/json",
        "Content-Type": "application/json"
      },
      validateStatus: function (status) {
        return status < 500;
      }
    });
  }

  static getMember = async (userId: string): Promise<DiscordMemberDto | null> => {
    const result = await DiscordMembersApi.init().get(`/${userId}`);
    if (result.status !== 200){
      return null;
    }
    return result.data;
  }

  static getMembers = async (usersIds: string[]): Promise<DiscordMemberDto[]> => {
    const result = await DiscordMembersApi.init().post(`/getMany`, usersIds);
    return result.data;
  }
}