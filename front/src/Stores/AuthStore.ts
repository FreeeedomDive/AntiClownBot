import { makeAutoObservable } from "mobx";
import { Cookies } from "react-cookie";

const cookies = new Cookies();

export class AuthStore {
  userId: string | undefined;

  constructor() {
    makeAutoObservable(this);
    this.userId = cookies.get("userId");
  }

  setUserId(userId: string) {
    cookies.set("userId", userId);
    this.userId = userId;
  }

  logOut() {
    cookies.remove("userId");
    this.userId = undefined;
  }
}

export const authStore = new AuthStore();
