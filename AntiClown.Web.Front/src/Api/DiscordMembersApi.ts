import axios from "axios";
import {DiscordMemberDto} from "../Dto/Users/DiscordMemberDto";

export default class DiscordMembersApi {
  static init = () => {
    return axios.create({
      baseURL: `/discordApi/members/`, timeout: 10000, headers: {
        Accept: "application/json",
        "Content-Type": "application/json"
      },
      validateStatus: function (status) {
        return status < 500;
      }
    });
  }

  static getMember = async (userId: string): Promise<DiscordMemberDto | undefined> => {
    const result = await DiscordMembersApi.init().get(`/${userId}`);
    return result.data;
  }
}