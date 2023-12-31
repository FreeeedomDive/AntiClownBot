import axios from "axios";
import {UserDto} from "../Dto/Users/UserDto";

export default class UsersApi {
  static init = () => {
    return axios.create({
      baseURL: `/api/users`, timeout: 10000, headers: {
        Accept: "application/json",
        "Content-Type": "application/json"
      },
      validateStatus: function (status) {
        return status < 500;
      }
    });
  }

  static get = async (userId: string): Promise<UserDto | undefined> => {
    const result = await UsersApi.init().get<UserDto>(`/${userId}`);
    if (result.status === 404) {
      return undefined;
    }
    return result.data;
  }
}