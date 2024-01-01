import axios from "axios";

export default class TokensApi {
  static init = () => {
    return axios.create({
      baseURL: `/dataApi/tokens/`, timeout: 10000, headers: {
        Accept: "application/json",
        "Content-Type": "application/json"
      },
      validateStatus: function (status) {
        return status < 500;
      }
    });
  }

  static isTokenValid = async (userId: string, token: string): Promise<boolean> => {
    const result = await TokensApi.init().post(`/${userId}/validate`, token);
    return result.status === 204;
  }
}