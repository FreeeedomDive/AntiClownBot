import { createContext, useContext } from "react";
import { authStore } from "./AuthStore";
import { rightsStore } from "./RightsStore";

const store = {
  authStore: authStore,
  rightsStore: rightsStore,
};

export const StoreContext = createContext(store);

export const useStore = () => {
  return useContext<typeof store>(StoreContext);
};