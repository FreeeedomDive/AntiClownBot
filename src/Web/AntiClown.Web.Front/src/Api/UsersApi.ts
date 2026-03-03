import axios from "axios";
import {UserDto} from "../Dto/Users/UserDto";
import {UserFilterDto} from "../Dto/Users/UserFilterDto";

export default class UsersApi {
  static init = () => {
    return axios.create({
      baseURL: `/webApi/users`, timeout: 10000, headers: {
        Accept: "application/json",
        "Content-Type": "application/json"
      },
      validateStatus: function (status) {
        return status < 500;
      }
    });
  }

  static get = async (userId: string): Promise<UserDto | null> => {
    const result = await UsersApi.init().get<UserDto>(`/${userId}`);
    if (result.status === 404) {
      return null;
    }
    return result.data;
  }

  static find = async (filter: UserFilterDto): Promise<UserDto | null> => {
    const result = await UsersApi.init().post<UserDto | null>(`/find`, filter);
    return result.data;
  }
}