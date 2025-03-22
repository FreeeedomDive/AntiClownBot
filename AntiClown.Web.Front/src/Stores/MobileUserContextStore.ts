import {makeAutoObservable} from "mobx";
import {UserDto} from "../Dto/Users/UserDto";
import {DiscordMemberDto} from "../Dto/Users/DiscordMemberDto";

export class MobileUserContextStore {
  isUnknown: boolean;
  telegramUser: WebAppUser | null;
  user: UserDto | null;
  discordMember: DiscordMemberDto | null;

  constructor() {
    makeAutoObservable(this);
    this.isUnknown = true;
    this.telegramUser = null;
    this.user = null;
    this.discordMember = null;
  }

  addTelegramUser(user: WebAppUser) {
    this.telegramUser = user;
  }

  addUser(user: UserDto) {
    this.user = user;
    this.isUnknown = false;
  }

  addDiscordMember(discordMember: DiscordMemberDto) {
    this.discordMember = discordMember;
    this.isUnknown = false;
  }
}

export const mobileUserContextStore = new MobileUserContextStore();