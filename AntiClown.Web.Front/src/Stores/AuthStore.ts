import { makeAutoObservable } from "mobx";
import { Cookies } from "react-cookie";

const cookiesOptions = {
  path: "/",
};

const cookies = new Cookies(null, cookiesOptions);

export class AuthStore {
  loggedInUserId: string | undefined;
  userToken: string | undefined;
  telegramUserId: number | undefined;

  constructor() {
    makeAutoObservable(this);
    this.loggedInUserId = cookies.get("userId", {
      doNotParse: true,
    });
    this.userToken = cookies.get("token", {
      doNotParse: true,
    });
  }

  logInViaTelegram(telegramUserId: number | undefined){
    cookies.set("telegramUserId", telegramUserId, cookiesOptions);
    this.telegramUserId = telegramUserId;
  }

  logIn(userId: string, token: string) {
    cookies.set("userId", userId, cookiesOptions);
    cookies.set("token", token, cookiesOptions);
    this.loggedInUserId = userId;
    this.userToken = token;
  }

  logOut() {
    cookies.remove("userId", cookiesOptions);
    cookies.remove("token", cookiesOptions);
    this.loggedInUserId = undefined;
    this.userToken = undefined;
  }
}

export const authStore = new AuthStore();
