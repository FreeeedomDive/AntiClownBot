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
    const telegramUserId = cookies.get("telegramUserId", {
      doNotParse: true,
    });
    this.telegramUserId = telegramUserId
      ? Number(telegramUserId)
      : undefined;
  }

  logInViaTelegram(telegramUserId: number, userId?: string) {
    cookies.set("telegramUserId", telegramUserId, cookiesOptions);
    this.telegramUserId = telegramUserId;
    if (userId) {
      cookies.set("userId", userId, cookiesOptions);
      this.loggedInUserId = userId;
    }
  }

  logIn(userId: string, token: string) {
    cookies.remove("telegramUserId", cookiesOptions);
    cookies.set("userId", userId, cookiesOptions);
    cookies.set("token", token, cookiesOptions);
    this.telegramUserId = undefined;
    this.loggedInUserId = userId;
    this.userToken = token;
  }

  logOut() {
    cookies.remove("userId", cookiesOptions);
    cookies.remove("token", cookiesOptions);
    cookies.remove("telegramUserId", cookiesOptions);
    this.loggedInUserId = undefined;
    this.userToken = undefined;
    this.telegramUserId = undefined;
  }
}

export const authStore = new AuthStore();
