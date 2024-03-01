import { makeAutoObservable } from "mobx";
import { Cookies } from "react-cookie";

const cookiesOptions = {
  path: "/",
};

const cookies = new Cookies(null, cookiesOptions);

export class AuthStore {
  loggedInUserId: string | undefined;

  constructor() {
    makeAutoObservable(this);
    this.loggedInUserId = cookies.get("userId", {
      doNotParse: true,
    });
  }

  logIn(userId: string) {
    cookies.set("userId", userId, cookiesOptions);
    this.loggedInUserId = userId;
  }

  logOut() {
    cookies.remove("userId", cookiesOptions);
    this.loggedInUserId = undefined;
  }
}

export const authStore = new AuthStore();
