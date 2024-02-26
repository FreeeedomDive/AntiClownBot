import { makeAutoObservable } from "mobx";
import { Cookies } from "react-cookie";

const cookies = new Cookies({}, {
  path: "/"
});

export class AuthStore {
  loggedInUserId: string | undefined;

  constructor() {
    makeAutoObservable(this);
    this.loggedInUserId = cookies.get("userId", {
      doNotParse: true,
    });
  }

  logIn(userId: string) {
    cookies.set("userId", userId);
    this.loggedInUserId = userId;
  }

  logOut() {
    cookies.remove("userId");
    this.loggedInUserId = undefined;
  }
}

export const authStore = new AuthStore();
