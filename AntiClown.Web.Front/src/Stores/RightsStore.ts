import {makeAutoObservable} from "mobx";
import {RightsDto} from "../Dto/Rights/RightsDto";

export class RightsStore {
  userRights: RightsDto[];

  constructor() {
    makeAutoObservable(this);
    this.userRights = [];
  }

  setRights(rights: RightsDto[]) {
    this.userRights = rights;
  }
}

export const rightsStore = new RightsStore();
